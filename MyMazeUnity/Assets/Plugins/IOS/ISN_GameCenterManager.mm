#import "ISN_GameCenterManager.h"
#import "ISN_NativeUtility.h"



NSString * const UNITY_SPLITTER = @"|";
NSString * const UNITY_SPLITTER2 = @"|%|";
//NSString * const UNITY_EOF = @"endofline";


@implementation ISN_GameCenterManager

static NSMutableDictionary *loadedPlayers;

static NSArray *loadedLeaderboardsSets;
static NSMutableDictionary* receivedInvites = nil;
static GKGameCenterViewController* leaderbaordsView;
static GKAchievementViewController* achievementView;




static ISN_GameCenterManager *_sharedInstance;

@synthesize earnedAchievementCache;
@synthesize delegate;


#pragma init


+ (id)sharedInstance {
    if (_sharedInstance == nil)  {
        _sharedInstance = [[self alloc] init];
    }
    
    return _sharedInstance;
}



- (id)init {
    self = [super init];
    if (self) {
        requestedLeaderboardId = NULL;
        earnedAchievementCache= NULL;
        isAchievementsWasLoaded = FALSE;
        loadedPlayers = [[NSMutableDictionary alloc] init];
        
        #if UNITY_VERSION < 500
            [loadedPlayers retain];
        #endif
        
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(applicationDidEnterBackground:) name:UIApplicationDidEnterBackgroundNotification object:nil];
        
        NSLog(@"ISN IOSGameCenterManager initialized");
    }
    
    return self;
}



- (void)dealloc {
    self.earnedAchievementCache= NULL;
#if UNITY_VERSION < 500
    [super dealloc];
#endif
}

- (void) authenticateLocalPlayer {
    NSLog(@"ISN authenticateLocalPlayer call");
    
    if([self isGameCenterAvailable]){
        GKLocalPlayer *localPlayer = [GKLocalPlayer localPlayer];
        
        if(localPlayer.authenticated == NO) {
            
            
            // [localPlayer setAuthenticateHandler:^(UIViewController *viewcontroller, NSError *error) {
            [localPlayer authenticateWithCompletionHandler:^(NSError *error){ //OLD Code
                if (localPlayer.isAuthenticated){
                    
                    NSMutableString * data = [[NSMutableString alloc] init];
                    
                    if(localPlayer.playerID != nil) {
                        [data appendString:localPlayer.playerID];
                    } else {
                        [data appendString:@""];
                    }
                    [data appendString:@","];
                    
                    
                    if(localPlayer.displayName != nil) {
                        [data appendString:localPlayer.displayName];
                    } else {
                        [data appendString:@""];
                    }
                    [data appendString:@","];
                    
                    
                    if(localPlayer.alias != nil) {
                        [data appendString:localPlayer.alias];
                    } else {
                        [data appendString:@""];
                    }
                    
                    
                    
                    NSString *str = [data copy];
                    
#if UNITY_VERSION < 500
                    [str autorelease];
#endif
                    
                    
                    UnitySendMessage("GameCenterManager", "onAuthenticateLocalPlayer", [ISNDataConvertor NSStringToChar:str]);
                    
                    NSLog(@"ISN PLAYER AUTHENTICATED %@", localPlayer.alias);
                    
                    
                    [[ISN_GameCenterRTM sharedInstance] initNotificationHandler];
                    [[ISN_GameCenterRTM sharedInstance] setVc:UnityGetGLViewController()];
                    [[ISN_GameCenterTBM sharedInstance] setVc:UnityGetGLViewController()];
                    
                    [[ISN_GameCenterListner sharedInstance] subscribe];
                    
                    if(!isAchievementsWasLoaded) {
                        [self loadAchievements];
                    }
                    
                    [self savePlayerInfo:localPlayer];
                    
                } else {
     
                    NSLog(@"ISN PLAYER NOT AUTHENTICATED");
                    if(error != nil) {
                        UnitySendMessage("GameCenterManager", "onAuthenticationFailed", [ISNDataConvertor serializeError:error]);
                        NSLog(@"ISN Error descr: %@", error.description);
                    } else {
                         UnitySendMessage("GameCenterManager", "onAuthenticationFailed", [ISNDataConvertor NSStringToChar:@""]);
                    }
                  
                   
                }
            }];
        } else {
            NSLog(@"ISN Do nothing - Player is already authenticated");
        }
    }
}



-(void) getSignature {
    GKLocalPlayer *localPlayer = [GKLocalPlayer localPlayer];
    [localPlayer generateIdentityVerificationSignatureWithCompletionHandler:^(NSURL *publicKeyUrl, NSData *signature, NSData *salt, uint64_t timestamp, NSError *error) {
        
        if(error != nil) {
            
            NSLog(@"error: %@", error.description);
            
            NSMutableString * ErrorData = [[NSMutableString alloc] init];
            [ErrorData appendFormat:@"%i", error.code];
            [ErrorData appendString:@"| "];
            [ErrorData appendString:error.description];
            
            
            
            
            NSString *ErrorDataString = [ErrorData copy];
#if UNITY_VERSION < 500
            [ErrorDataString autorelease];
#endif
            UnitySendMessage("GameCenterManager", "VerificationSignatureRetrieveFailed", [ISNDataConvertor NSStringToChar:ErrorDataString]);
            return;
        }
        
        NSMutableString *sig = [[NSMutableString alloc] init];
        const char *db = (const char *) [signature bytes];
        for (int i = 0; i < [signature length]; i++) {
            if(i != 0) {
                [sig appendFormat:@","];
            }
            
            [sig appendFormat:@"%i", (unsigned char)db[i]];
        }
        
        
        NSMutableString *slt = [[NSMutableString alloc] init];
        const char *db2 = (const char *) [salt bytes];
        for (int i = 0; i < [salt length]; i++) {
            if(i != 0) {
                [slt appendFormat:@","];
            }
            
            [slt appendFormat:@"%i", (unsigned char)db2[i]];
        }
        
        
        
        
        
        NSString *path = [[NSString alloc] initWithString:[publicKeyUrl absoluteString]];
        
        
        
        
        
        NSMutableString * array = [[NSMutableString alloc] init];
        [array appendString:path];
        [array appendString:@"| "];
        [array appendString:sig];
        [array appendString:@"| "];
        [array appendString:slt];
        [array appendString:@"| "];
        [array appendFormat:@"%llu", timestamp];
        
        
        NSString *str = [array copy];
#if UNITY_VERSION < 500
        [str autorelease];
#endif
        UnitySendMessage("GameCenterManager", "VerificationSignatureRetrieved", [ISNDataConvertor NSStringToChar:str]);
        
        
        
    }];
}





#pragma leaderboards

- (void) showLeaderboardsPopUp {
    
    leaderbaordsView = [[GKGameCenterViewController alloc] init];
    leaderbaordsView.gameCenterDelegate = self;
    
    
    CGSize screenSize = [[UIScreen mainScreen] bounds].size;
    
    UIViewController *vc =  UnityGetGLViewController();
    
    [vc presentViewController: leaderbaordsView animated: YES completion:nil];
    leaderbaordsView.view.transform = CGAffineTransformMakeRotation(0.0f);
    [leaderbaordsView.view setCenter:CGPointMake(screenSize.width/2, screenSize.height/2)];
    leaderbaordsView.view.bounds = CGRectMake(0, 0, screenSize.width, screenSize.height);
    
    
}

- (void) showLeaderboardPopUp {
    
    leaderbaordsView = [[GKGameCenterViewController alloc] init];
    if (leaderbaordsView != NULL)
    {
        
        leaderbaordsView.leaderboardCategory = requestedLeaderboardId;
        
        switch (lbscope) {
            case 2:
                leaderbaordsView.leaderboardTimeScope = GKLeaderboardTimeScopeAllTime;
                break;
            case 1:
                leaderbaordsView.leaderboardTimeScope = GKLeaderboardTimeScopeWeek;
                break;
            case 0:
                leaderbaordsView.leaderboardTimeScope = GKLeaderboardTimeScopeToday;
                break;
                
            default:
                leaderbaordsView.leaderboardTimeScope = GKLeaderboardTimeScopeAllTime;
                break;
        }
        
        leaderbaordsView.gameCenterDelegate = self;
        
        
        CGSize screenSize = [[UIScreen mainScreen] bounds].size;
        
        
        UIViewController *vc =  UnityGetGLViewController();
        
        [vc presentViewController: leaderbaordsView animated: YES completion:nil];
        leaderbaordsView.view.transform = CGAffineTransformMakeRotation(0.0f);
        [leaderbaordsView.view setCenter:CGPointMake(screenSize.width/2, screenSize.height/2)];
        leaderbaordsView.view.bounds = CGRectMake(0, 0, screenSize.width, screenSize.height);
        
#if UNITY_VERSION < 500
        [requestedLeaderboardId release];
#endif
        
        
    }
    
    requestedLeaderboardId = NULL;
}


-(void) showLeaderboard:(NSString *)leaderboardId scope:(int)scope {
    
    GKLocalPlayer *localPlayer = [GKLocalPlayer localPlayer];
    if(localPlayer.isAuthenticated) {
        
        requestedLeaderboardId = leaderboardId;
        
#if UNITY_VERSION < 500
        [requestedLeaderboardId retain];
#endif
        
        
        lbscope = scope;
        [self showLeaderboardPopUp];
    } else {
        NSLog(@"ISN showLeaderboard requires player to be authenticated.  Call ignored");
    }
}



- (void) reportScore: (long long) score forCategory: (NSString*) category {
    NSLog(@"ISN reportScore: %lld", score);
    NSLog(@"ISN category %@", category);
    
    
    
    GKScore *scoreReporter = [[GKScore alloc] initWithCategory:category] ;
#if UNITY_VERSION < 500
    [scoreReporter autorelease];
#endif
    scoreReporter.value = score;
    
    [scoreReporter reportScoreWithCompletionHandler: ^(NSError *error) {
        if (error != nil) {
            UnitySendMessage("GameCenterManager", "onScoreSubmittedFailed", [ISNDataConvertor serializeError:error] );
            NSLog(@"ISN Error while submitting score: %@", error.description);
        } else {
            NSLog(@"ISN new high score sumbitted success: %lld", score);
            
            
            
            NSMutableString * data = [[NSMutableString alloc] init];
            
            
            [data appendString:category];
            [data appendString:@","];
            [data appendString:[NSString stringWithFormat: @"%lld", score]];
            
            
            NSString *str = [data copy];
            
#if UNITY_VERSION < 500
            [str autorelease];
#endif
            UnitySendMessage("GameCenterManager", "onScoreSubmittedEvent", [ISNDataConvertor NSStringToChar:str]);
            
            
        }
        
    }];
}

-(void) retriveScores:(NSString *)category scope:(int)scope collection: (int) collection from:(int)from to:(int)to {
    GKLeaderboard *board = [[GKLeaderboard alloc] init];
    
    if(board != nil) {
        
        
        board.range = NSMakeRange(from, to);
        board.category = category;
        switch (scope) {
            case 2:
                board.timeScope = GKLeaderboardTimeScopeAllTime;
                break;
            case 1:
                board.timeScope = GKLeaderboardTimeScopeWeek;
                break;
            case 0:
                board.timeScope = GKLeaderboardTimeScopeToday;
                break;
                
            default:
                board.timeScope = GKLeaderboardTimeScopeAllTime;
                break;
        }
        
        switch (collection) {
            case 1:
                board.playerScope = GKLeaderboardPlayerScopeGlobal;
                break;
            case 0:
                board.playerScope = GKLeaderboardPlayerScopeFriendsOnly;
                break;
                
            default:
                board.playerScope = GKLeaderboardPlayerScopeFriendsOnly;
                break;
        }
        
        
        [board loadScoresWithCompletionHandler: ^(NSArray *scores, NSError *error) {
            if (error != nil) {
                // handle the error.
                NSLog(@"ISN Error retrieving score: %@", error.description);
                
                UnitySendMessage("GameCenterManager", "onLeaderboardScoreListLoadFailed", [ISNDataConvertor serializeError:error]);
                return;
                
            }
            
            
            if (scores != nil) {
                
                NSMutableString * data = [[NSMutableString alloc] init];
                [data appendString:category];
                [data appendString:@","];
                [data appendString:[NSString stringWithFormat:@"%d", scope]];
                [data appendString:@","];
                [data appendString:[NSString stringWithFormat:@"%d", collection]];
                [data appendString:@","];
                
                
                BOOL first = YES;
                
                NSEnumerator *e = [scores objectEnumerator];
                id object;
                while (object = [e nextObject]) {
                    GKScore* s =((GKScore*) object);
                    
                    
                    if(!first) {
                        [data appendString:@","];
                    }
                    
                    [data appendString:s.playerID];
                    [data appendString:@","];
                    
                    [data appendString:[NSString stringWithFormat:@"%lld", s.value]];
                    [data appendString:@","];
                    
                    [data appendString:[NSString stringWithFormat:@"%d", s.rank]];
                    
                    [self savePlayerInfo:s.player];
                    
                    first = NO;
                    
                }
                
                
                NSMutableString * rangeData = [[NSMutableString alloc] init];
                [rangeData appendString:[NSString stringWithFormat:@"%@", board.identifier]];
                [rangeData appendString:@"|"];
                [rangeData appendString:[NSString stringWithFormat:@"%d", board.maxRange]];
                
                
                UnitySendMessage("GameCenterManager", "OnLeaderboardMaxRangeUpdate", [ISNDataConvertor NSStringToChar:rangeData]);
                
                
                UnitySendMessage("GameCenterManager", "onLeaderboardScoreListLoaded", [ISNDataConvertor NSStringToChar:data]);
                
                
                
            } else {
                UnitySendMessage("GameCenterManager", "onLeaderboardScoreListLoadFailed", [ISNDataConvertor NSStringToChar:@""]);
            }
            
        }];
    }
    
#if UNITY_VERSION < 500
    [board release];
#endif

}


- (void) sendLeaderboardChallengeWithFriendsPicker:(NSString *)leaderboardId message:(NSString *)message {
    
    
    
    GKLeaderboard *leaderboardRequest = [[GKLeaderboard alloc] init];
#if UNITY_VERSION < 500
    [leaderboardRequest autorelease];
#endif
    
    leaderboardRequest.category = leaderboardId;
    
    leaderboardRequest.timeScope = GKLeaderboardTimeScopeAllTime;

    
    
    if (leaderboardRequest != nil) {
        
        [leaderboardRequest loadScoresWithCompletionHandler:^(NSArray *scores, NSError *error){
            if (error != nil) {
                NSLog(@"ISN Error challenge scores loading %@", error.description);
            }  else {
                
                UIViewController *composeVC = [leaderboardRequest.localPlayerScore challengeComposeControllerWithPlayers:nil message:message completionHandler:^(UIViewController *composeController, BOOL didIssueChallenge, NSArray *sentPlayerIDs){
                    
                    
                    [composeController dismissViewControllerAnimated:YES completion:nil];
                    
                    [composeController.view.superview removeFromSuperview];
                    
                }];
                
                
                UIViewController *vc =  UnityGetGLViewController();
                
                [vc presentViewController: composeVC animated: YES completion:nil];
                
                
                
                
            }
        }];
    }
    
}


-(void) sendLeaderboardChallenge:(NSString *)leaderboardId message:(NSString *)message playerIds:(NSArray *)playerIds {
    
    
    
    GKLeaderboard *leaderboardRequest = [[GKLeaderboard alloc] init];
#if UNITY_VERSION < 500
    [leaderboardRequest autorelease];
#endif
    leaderboardRequest.category = leaderboardId;
    
    leaderboardRequest.timeScope = GKLeaderboardTimeScopeAllTime;
    
    
    if (leaderboardRequest != nil) {
        
        [leaderboardRequest loadScoresWithCompletionHandler:^(NSArray *scores, NSError *error){
            if (error != nil) {
                NSLog(@"ISN Error challenge scores loading");
            }  else {
                
                [leaderboardRequest.localPlayerScore issueChallengeToPlayers:playerIds message:message];
                
            }
        }];
    }
    
}


-(void) retrieveScoreForLocalPlayerWithCategory:(NSString *)category scope:(int)scope collection:(int)collection{
    
    GKLeaderboard *leaderboardRequest = [[GKLeaderboard alloc] init];
#if UNITY_VERSION < 500
    [leaderboardRequest autorelease];
#endif
    leaderboardRequest.category = category;
    
    
    switch (scope) {
        case 2:
            leaderboardRequest.timeScope = GKLeaderboardTimeScopeAllTime;
            break;
        case 1:
            leaderboardRequest.timeScope = GKLeaderboardTimeScopeWeek;
            break;
        case 0:
            leaderboardRequest.timeScope = GKLeaderboardTimeScopeToday;
            break;
            
        default:
            leaderboardRequest.timeScope = GKLeaderboardTimeScopeAllTime;
            break;
    }
    
    switch (collection) {
        case 1:
            leaderboardRequest.playerScope = GKLeaderboardPlayerScopeGlobal;
            break;
        case 0:
            leaderboardRequest.playerScope = GKLeaderboardPlayerScopeFriendsOnly;
            break;
            
        default:
            leaderboardRequest.playerScope = GKLeaderboardPlayerScopeFriendsOnly;
            break;
    }
    
    
    
    if (leaderboardRequest != nil) {
        NSMutableString * data = [[NSMutableString alloc] init];
        
        [leaderboardRequest loadScoresWithCompletionHandler:^(NSArray *scores, NSError *error){
            if (error != nil) {
                NSLog(@"ISN Error scores loading %@", error.description);
                UnitySendMessage("GameCenterManager", "onLeaderboardScoreFailed", [ISNDataConvertor serializeError:error]);
                
            }  else {
                [data appendString:category];
                [data appendString:@","];
                
                
                
                [data appendString:[NSString stringWithFormat:@"%lld", leaderboardRequest.localPlayerScore.value]];
                [data appendString:@","];
                [data appendString:[NSString stringWithFormat:@"%d", leaderboardRequest.localPlayerScore.rank]];
                [data appendString:@","];
                [data appendString:[NSString stringWithFormat:@"%d", scope]];
                [data appendString:@","];
                [data appendString:[NSString stringWithFormat:@"%d", collection]];
                
                
                NSString *str = [data copy];
#if UNITY_VERSION < 500
                [str autorelease];
#endif
                
                
                NSMutableString * rangeData = [[NSMutableString alloc] init];
                [rangeData appendString:[NSString stringWithFormat:@"%@", leaderboardRequest.identifier]];
                [rangeData appendString:@"|"];
                [rangeData appendString:[NSString stringWithFormat:@"%d", leaderboardRequest.maxRange]];
                
                
                NSString *rangeStr = [rangeData copy];
#if UNITY_VERSION < 500
                [rangeStr autorelease];
#endif
                UnitySendMessage("GameCenterManager", "OnLeaderboardMaxRangeUpdate", [ISNDataConvertor NSStringToChar:rangeStr]);
                
                
                
                
                
                UnitySendMessage("GameCenterManager", "onLeaderboardScore", [ISNDataConvertor NSStringToChar:str]);
                
                
                
                
                NSLog(@"ISN Retrieved localScore:%lld",leaderboardRequest.localPlayerScore.value);
                NSLog(@"ISN MaxRange %lu", (unsigned long)leaderboardRequest.maxRange);
            }
        }];
    }
}

-(void) loadLeaderboardSetInfo {
    [GKLeaderboardSet loadLeaderboardSetsWithCompletionHandler:^(NSArray *leaderboardSets, NSError *error) {
        
        if(error != NULL) {
            NSLog(@"ISN Error loadLeaderboardSetInfo loading %@", error.description);
            UnitySendMessage("GameCenterManager", "ISN_OnLBSetsLoadFailed", "");
        } else {
            NSMutableString * data = [[NSMutableString alloc] init];
            BOOL first = YES;
            for (GKLeaderboardSet *lb in leaderboardSets) {
                if(!first) {
                    [data appendString:@"|"];
                }
                first = NO;
                [data appendString:lb.title];
                [data appendString:@"|"];
                [data appendString:lb.identifier];
                [data appendString:@"|"];
                [data appendString:lb.groupIdentifier];
                
                
            }
            
            
            loadedLeaderboardsSets = leaderboardSets;
            
            
            
            NSString *str = [data copy];
#if UNITY_VERSION < 500
            [str autorelease];
#endif
            UnitySendMessage("GameCenterManager", "ISN_OnLBSetsLoaded", [ISNDataConvertor NSStringToChar:str]);
        }
        
    }];
}


-(void) loadLeaderboardsForSet:(NSString *)uid {
    for (GKLeaderboardSet *lb in loadedLeaderboardsSets) {
        if([lb.identifier isEqualToString:uid] ) {
            
            [lb loadLeaderboardsWithCompletionHandler:^(NSArray *leaderboards, NSError *error) {
                if(error != NULL) {
                    NSLog(@"ISN Error loadLeaderboardsWithCompletionHandler loading %@", error.description);
                    UnitySendMessage("GameCenterManager", "ISN_OnLBSetsBoardsLoadFailed", [ISNDataConvertor NSStringToChar:lb.identifier]);
                } else {
                    
                    
                    NSMutableString * data = [[NSMutableString alloc] init];
                    [data appendString:lb.identifier];
                    
                    BOOL first = YES;
                    for (GKLeaderboard *l in leaderboards) {
                        if(!first) {
                            [data appendString:@"|"];
                        }
                        first = NO;
                        [data appendString:l.title];
                        [data appendString:@"|"];
                        [data appendString:l.description];
                        [data appendString:@"|"];
                        [data appendString:l.identifier];
                        
                        
                    }
                    
                    NSString *str = [data copy];
#if UNITY_VERSION < 500
                    [str autorelease];
#endif
                    
                    UnitySendMessage("GameCenterManager", "ISN_OnLBSetsBoardsLoaded", [ISNDataConvertor NSStringToChar:str]);
                    
                }
                
            }];
            
            return;
        }
    }
    
    
}



#pragma achivments


-(void) resetAchievements {
    self.earnedAchievementCache= NULL;
    [GKAchievement resetAchievementsWithCompletionHandler: ^(NSError *error)  {
        
        if(error != nil) {
            NSLog(@"ISN resetAchievements failed: %@", error.description);
            UnitySendMessage("GameCenterManager", "onAchievementsResetFailed", [ISNDataConvertor serializeError:error]);
        } else {
            NSLog(@"ISN resetAchievements complete");
            UnitySendMessage("GameCenterManager", "onAchievementsReset", "");
            
        }
        
    }];
}



-(void) submitAchievement:(double)percentComplete identifier:(NSString *)identifier  notifyComplete: (BOOL) notifyComplete {
    //GameCenter check for duplicate achievements when the achievement is submitted, but if you only want to report
    // new achievements to the user, then you need to check if it's been earned
    // before you submit.  Otherwise you'll end up with a race condition between loadAchievementsWithCompletionHandler
    // and reportAchievementWithCompletionHandler.  To avoid this, we fetch the current achievement list once,
    // then cache it and keep it updated with any new achievements.
    
    NSLog(@"ISN submitAchievement %@", identifier);
    
    if(self.earnedAchievementCache == NULL) {
        NSLog(@"ISN loading Achievements cache....");
        [GKAchievement loadAchievementsWithCompletionHandler: ^(NSArray *scores, NSError *error) {
            if(error == NULL)  {
                NSMutableDictionary* tempCache= [NSMutableDictionary dictionaryWithCapacity: [scores count]];
                for (GKAchievement* score in tempCache) {
                    [tempCache setObject: score forKey: score.identifier];
                }
                
                
                
                
                self.earnedAchievementCache= tempCache;
                NSLog(@"ISN Achievements cache loaded, resubmitting achievement");
                [self submitAchievement:percentComplete identifier:identifier notifyComplete:notifyComplete];
                
            }
            else{
                NSLog(@"ISN Achievements cache load error: %@", error.description);
            }
            
        }];
    } else {
        //Search the list for the ID we're using...
        GKAchievement* achievement= [self.earnedAchievementCache objectForKey: identifier];
        if(achievement != NULL) {
            if((achievement.percentComplete >= 100.0) || (achievement.percentComplete >= percentComplete)) {
                //Achievement has already been earned so we're done.
                achievement= NULL;
            }
            
            achievement.percentComplete= percentComplete;
        } else {
            
            achievement= [[GKAchievement alloc] initWithIdentifier: identifier];
#if UNITY_VERSION < 500
            [achievement autorelease];
#endif
            achievement.showsCompletionBanner = notifyComplete;
            
            if(percentComplete > 100.0) {
                percentComplete = 100.0;
            }
            
            achievement.percentComplete= percentComplete;
            
            //Add achievement to achievement cache...
            [self.earnedAchievementCache setObject: achievement forKey: achievement.identifier];
        }
        
        if(achievement!= NULL) {
            NSLog(@"ISN Submit Achievement");
            //Submit the Achievement...
            [achievement reportAchievementWithCompletionHandler: ^(NSError *error) {
                if(error == NULL) {
                    NSMutableString * data = [[NSMutableString alloc] init];
                    [data appendString:achievement.identifier];
                    [data appendString:@","];
                    [data appendString:[NSString stringWithFormat:@"%f", achievement.percentComplete]];
                    
                    
                    NSString *str = [data copy];
#if UNITY_VERSION < 500
                    [str autorelease];
#endif
                    
                    NSLog(@"ISN Submitted");
                    NSLog(@"ISN identifier: %@", achievement.identifier);
                    NSLog(@"ISN percentComplete: %f", achievement.percentComplete);
                    
                    UnitySendMessage("GameCenterManager", "onAchievementProgressChanged", [ISNDataConvertor NSStringToChar:str]);
                } else {
                    NSLog(@"ISN Submit failed with error %@", error.description);
                }
            }];
        }
    }

}



- (void) showAchievements {
    
    GKLocalPlayer *localPlayer = [GKLocalPlayer localPlayer];
    if(localPlayer.isAuthenticated) {
        [self showAchievementsPopUp];
    } else {
        NSLog(@"ISN showAchievements requires player to be authenticated.  Call ignored");
    }
}

- (void) showAchievementsPopUp {
    
    achievementView = [[GKAchievementViewController alloc] init];
    if(achievementView != NULL) {
        
        achievementView.achievementDelegate = self;
        
        CGSize screenSize = [[UIScreen mainScreen] bounds].size;
        UIViewController *vc =  UnityGetGLViewController();
        [vc presentViewController: achievementView animated: YES completion:nil];
        
        achievementView.view.transform = CGAffineTransformMakeRotation(0.0f);
        [achievementView.view setCenter:CGPointMake(screenSize.width/2, screenSize.height/2)];
        achievementView.view.bounds = CGRectMake(0, 0, screenSize.width, screenSize.height);
    }
}


- (void) loadAchievements {
    [GKAchievement loadAchievementsWithCompletionHandler:^(NSArray *achievements, NSError *error) {
        if (error == nil) {
            NSLog(@"ISN loadAchievementsWithCompletionHandler");
            NSLog(@"ISN count %lu", (unsigned long)achievements.count);
            
            
            isAchievementsWasLoaded = TRUE;
            NSMutableString * data = [[NSMutableString alloc] init];
            BOOL first = YES;
            for (GKAchievement* achievement in achievements) {
                
                
                if(!first) {
                    [data appendString:@","];
                }
                
                
                [data appendString:achievement.identifier];
                [data appendString:@","];
                
                [data appendString:achievement.description];
                [data appendString:@","];
                
                
                NSLog(@"ISN achievement.percentComplete:  %f", achievement.percentComplete);
                
                [data appendString:[NSString stringWithFormat:@"%f", achievement.percentComplete]];
                
                first = NO;
                
            }
            
            NSString *str = [data copy];
            
#if UNITY_VERSION < 500
            [str autorelease];
#endif
            
            UnitySendMessage("GameCenterManager", "onAchievementsLoaded", [ISNDataConvertor NSStringToChar:str]);
        } else {
            NSLog(@"ISN loadAchievements failed:  %@", error.description);
            UnitySendMessage("GameCenterManager", "onAchievementsLoadedFailed",  [ISNDataConvertor serializeError:error]);
        }
    }];
}

-(void) sendAchievementChallengeWithFriendsPicker:(NSString *)achievementId message:(NSString *)message {
    GKAchievement *achievement = [[GKAchievement alloc] initWithIdentifier: achievementId];
    
#if UNITY_VERSION < 500
    [achievement autorelease];
#endif
    
    UIViewController *composeVC = [achievement challengeComposeControllerWithPlayers:nil message:message completionHandler:^(UIViewController *composeController, BOOL didIssueChallenge, NSArray *sentPlayerIDs) {
        [composeController dismissViewControllerAnimated:YES completion:nil];
        [composeController.view.superview removeFromSuperview];
    }];
    
    
    UIViewController *vc =  UnityGetGLViewController();
    [vc presentViewController: composeVC animated: YES completion:nil];
}


-(void) sendAchievementChallenge:(NSString *)achievementId message:(NSString *)message playerIds:(NSArray *)playerIds {
    GKAchievement *achievement = [[GKAchievement alloc] initWithIdentifier: achievementId];
#if UNITY_VERSION < 500
    [achievement autorelease];
#endif
    
    [achievement issueChallengeToPlayers:playerIds message:message];
}


#pragma invites

-(NSString* ) serialiseInvite:(GKInvite*) invite {
    NSString* inviteId = [self saveInvite:invite];
    
    [self savePlayerInfo:invite.sender];
    
    NSMutableString * data = [[NSMutableString alloc] init];

    [data appendString:inviteId];
    [data appendString: UNITY_SPLITTER];
    
    [data appendString:invite.sender.playerID];
    [data appendString: UNITY_SPLITTER];
    
    [data appendString: [NSString stringWithFormat:@"%d", invite.playerGroup]];
    [data appendString: UNITY_SPLITTER];
    
    [data appendString: [NSString stringWithFormat:@"%d", invite.playerAttributes]];
    
    return data;

}

- (NSString*) saveInvite:(GKInvite*) invite {
    if(receivedInvites == nil) {
        receivedInvites = [[NSMutableDictionary alloc] init];
    }
    
    NSString* inviteId =  [NSString stringWithFormat:@"%d", receivedInvites.count + 1];
    [receivedInvites setObject:invite forKey:inviteId];
    
    return inviteId;
}

- (GKInvite*) getInviteWithId:(NSString*)inviteId {
    GKInvite* invite = [receivedInvites objectForKey:inviteId];
    return  invite;
}


#pragma friends

-(GKPlayer*) getPlayerWithId:(NSString*) playerId {
    return [loadedPlayers objectForKey:playerId];
}


-(void) savePlayerInfo:(GKPlayer *)player {

    if([loadedPlayers objectForKey:player.playerID] != nil) {
        return;
    }
    
    [loadedPlayers setObject:player forKey:player.playerID];
    
    NSMutableString * data = [[NSMutableString alloc] init];
    [data appendString:player.playerID];
    [data appendString:UNITY_SPLITTER];
    [data appendString:player.alias];
    [data appendString:UNITY_SPLITTER];
    [data appendString:player.displayName];
    
    
    
    NSString *str = [data copy];
#if UNITY_VERSION < 500
    [str autorelease];
#endif
    UnitySendMessage("GameCenterManager", "OnUserInfoLoadedEvent", [ISNDataConvertor NSStringToChar:str]);
    
}

-(void) loadPlayerInfoForPlayerWithId:(NSString *)playerId {
    if([loadedPlayers objectForKey:playerId] == nil) {
        return;
    }
    
    NSArray *playerIdArray = [NSArray arrayWithObject:playerId];
    [GKPlayer loadPlayersForIdentifiers:playerIdArray withCompletionHandler:^(NSArray *players, NSError *error) {
        
        if (error != nil) {
            if(error.description != NULL) {
                NSLog(@"ISN Player failed to load: %@", error.description);
            } else {
                NSLog(@"ISN Player failed to load, no erro describtion provided");
            }
            
            UnitySendMessage("GameCenterManager", "OnUserInfoLoadFailedEvent", [ISNDataConvertor NSStringToChar:playerId]);
        } else {
            GKPlayer *player = [players objectAtIndex:0];
            [self savePlayerInfo:player];
        }
        
    }];

}


-(void) loadImageForPlayerWithPlayerId:(NSString *)playerId size:(GKPhotoSize)size {
    
    GKPlayer* player = [loadedPlayers objectForKey:playerId];
    
    if(player == nil) {
        NSLog(@"ISN player with id %@ not found in the loadedPlayers array", playerId);
        return;
    }
    
    [player loadPhotoForSize:size withCompletionHandler:^(UIImage *photo, NSError *error) {
        
        
        if(error != nil) {
            
            NSString* errorData = [ISNDataConvertor serializeErrorToNSString:error];
            
            NSMutableString * data = [[NSMutableString alloc] init];
            [data appendString:player.playerID];
            [data appendString:UNITY_SPLITTER2];
            [data appendString: [NSString stringWithFormat:@"%d", size]];
            [data appendString:UNITY_SPLITTER2];
            [data appendString:errorData];

            UnitySendMessage("GameCenterManager", "OnUserPhotoLoadFailedEvent", [ISNDataConvertor NSStringToChar:[data copy]]);

            return;
        }
        
        NSString *encodedImage = @"";
        
        
        if (photo == nil) {
            NSLog(@"ISN No photo for user with ID: %@", playerId);
        } else {
            NSData *imageData = UIImagePNGRepresentation(photo);
            encodedImage = [imageData base64Encoding];
        }
        
        
        NSMutableString * data = [[NSMutableString alloc] init];
        [data appendString:player.playerID];
        [data appendString:UNITY_SPLITTER];
        [data appendString: [NSString stringWithFormat:@"%d", size]];
        [data appendString:UNITY_SPLITTER];
        [data appendString:encodedImage];
        
        
        NSLog(@"ISN User Photo Loaded for player with ID:  %@", playerId);
        NSString *str = [data copy];
        
#if UNITY_VERSION < 500
        [str autorelease];
#endif
        UnitySendMessage("GameCenterManager", "OnUserPhotoLoadedEvent", [ISNDataConvertor NSStringToChar:str]);
        
    }];

}



-(void) retrieveFriends {
    GKLocalPlayer *localPlayer = [GKLocalPlayer localPlayer];
    if (localPlayer.authenticated) {
        
        if([ISN_NativeUtility majorIOSVersion] >= 8) {
            [self IOS8LoadFriends];
        } else  {
            [self OldIOSLoadFreinds];
        }
        
    } else {
        NSLog(@"ISN User friends cannot be loaded before player auth, sending fail event");
        UnitySendMessage("GameCenterManager", "OnFriendListLoadFailEvent", [ISNDataConvertor serializeErrorWithData:@"User has to be authenticated to perform friends load operation" code:0]);
    }
    
}

-(void) OldIOSLoadFreinds {
    GKLocalPlayer *localPlayer = [GKLocalPlayer localPlayer];
     [localPlayer loadFriendsWithCompletionHandler:^(NSArray *friends, NSError *error) {
     
         if(error != NULL) {
             UnitySendMessage("GameCenterManager", "OnFriendListLoadFailEvent", [ISNDataConvertor serializeError:error]);
             return;
         }
         
         
         if (friends != nil && friends.count > 0) {
             
             BOOL first = YES;
             NSMutableString * data = [[NSMutableString alloc] init];
             for (NSString *friendId in friends) {
                
                 [self loadPlayerInfoForPlayerWithId:friendId];
                 
                 if(!first) {
                     [data appendString:UNITY_SPLITTER];
                 }
                 first = NO;
                 
                 [data appendString:friendId];
                 
             }
             
             NSString *str = [data copy];
             
#if UNITY_VERSION < 500
             [str autorelease];
#endif
             
             
             UnitySendMessage("GameCenterManager", "OnFriendListLoadedEvent", [ISNDataConvertor NSStringToChar:str]);
         } else {
             UnitySendMessage("GameCenterManager", "OnFriendListLoadedEvent", [ISNDataConvertor NSStringToChar:@""]);
         }

         
     }];
}

-(void) IOS8LoadFriends {
    
    GKLocalPlayer *localPlayer = [GKLocalPlayer localPlayer];
    
    [localPlayer loadFriendPlayersWithCompletionHandler:^(NSArray *friendPlayers, NSError *error) {
        if(error != NULL) {
            UnitySendMessage("GameCenterManager", "OnFriendListLoadFailEvent", [ISNDataConvertor serializeError:error]);
            return;
        }
        
        if (friendPlayers != nil && friendPlayers.count > 0) {
            
            BOOL first = YES;
            NSMutableString * data = [[NSMutableString alloc] init];
            for (GKPlayer *userFriend in friendPlayers) {
                [self savePlayerInfo:userFriend];
                
                if(!first) {
                    [data appendString:UNITY_SPLITTER];
                }
                first = NO;
                
                [data appendString:userFriend.playerID];
                
            }
            
            NSString *str = [data copy];
            
#if UNITY_VERSION < 500
            [str autorelease];
#endif
            
            
            UnitySendMessage("GameCenterManager", "OnFriendListLoadedEvent", [ISNDataConvertor NSStringToChar:str]);
        } else {
            UnitySendMessage("GameCenterManager", "OnFriendListLoadedEvent", [ISNDataConvertor NSStringToChar:@""]);
        }
        
    }];

}


#pragma private methods

- (void)gameCenterViewControllerDidFinish:(GKGameCenterViewController *)vc {
    [self dismissGameCenterView:leaderbaordsView];
    leaderbaordsView =  nil;
}


- (void)achievementViewControllerDidFinish:(GKAchievementViewController *)vc; {
    [self dismissGameCenterView:achievementView];
    achievementView = nil;
}


-(void) dismissGameCenterView: (GKGameCenterViewController *)vc {
    
    if(vc == nil) {
        return;
    }
    
    if (![vc isBeingPresented] && ![vc isBeingDismissed]) {
        [vc dismissViewControllerAnimated:YES completion:nil];
       // [vc.view.superview removeFromSuperview];
    }
    
#if UNITY_VERSION < 500
    [vc release];
#endif
    
    vc = nil;
    
    
    UnitySendMessage("GameCenterManager", "OnGameCenterViewDismissedEvent", [ISNDataConvertor NSStringToChar:@""]);
}

- (void)applicationDidEnterBackground:(UIApplication*)application {
    
    NSLog(@"applicationDidEnterBackground");
    
    [self dismissGameCenterView:leaderbaordsView];
    leaderbaordsView = nil;
    
    
    [self dismissGameCenterView:achievementView];
    achievementView = nil;
}


- (BOOL)isGameCenterAvailable {
    BOOL localPlayerClassAvailable = (NSClassFromString(@"GKLocalPlayer")) != nil;
    NSString *reqSysVer = @"4.1";
    NSString *currSysVer = [[UIDevice currentDevice] systemVersion];
    BOOL osVersionSupported = ([currSysVer compare:reqSysVer options:NSNumericSearch] != NSOrderedAscending);
    return (localPlayerClassAvailable && osVersionSupported);
}


@end

extern "C" {
    void _initGameCenter ()  {
        [[ISN_GameCenterManager sharedInstance] authenticateLocalPlayer];
    }
    
    
    void _showLeaderboard(char* leaderboardId, int scope) {
        [[ISN_GameCenterManager sharedInstance] showLeaderboard:[ISNDataConvertor charToNSString:leaderboardId] scope:scope];
    }
    
    void _showLeaderboards() {
        [[ISN_GameCenterManager sharedInstance] showLeaderboardsPopUp];
    }
    
    void _getLeaderboardScore(char* leaderboardId, int scope, int collection) {
        [[ISN_GameCenterManager sharedInstance] retrieveScoreForLocalPlayerWithCategory:[ISNDataConvertor charToNSString:leaderboardId] scope:scope collection:collection];
    }
    
    void _loadLeaderboardScore(char* leaderboardId, int scope, int collection, int from, int to) {
        
        [[ISN_GameCenterManager sharedInstance] retriveScores:[ISNDataConvertor charToNSString:leaderboardId] scope:scope  collection: collection from:from to:to];
    }
    
    void _showAchievements() {
        //[GCManager authenticateLocalPlayer];
        [[ISN_GameCenterManager sharedInstance] showAchievements];
    }
    
    void _ISN_issueLeaderboardChallenge(char* leaderboardId, char* message, char* playerIds) {
        
        
        NSString* str = [ISNDataConvertor charToNSString:playerIds];
        NSArray *ids = [str componentsSeparatedByString:@","];
        
        [[ISN_GameCenterManager sharedInstance] sendLeaderboardChallenge:[ISNDataConvertor charToNSString:leaderboardId] message:[ISNDataConvertor charToNSString:message] playerIds:ids];
        
    }
    
    void _ISN_issueLeaderboardChallengeWithFriendsPicker(char* leaderboardId, char* message) {
        
        NSString* lid = [ISNDataConvertor charToNSString:leaderboardId];
        NSString* mes = [ISNDataConvertor charToNSString:message];
        
        [[ISN_GameCenterManager sharedInstance] sendLeaderboardChallengeWithFriendsPicker:lid message:mes];
        
    }
    
    
    void _ISN_issueAchievementChallenge(char* achievementId, char* message, char* playerIds) {
        
        NSString* str = [ISNDataConvertor charToNSString:playerIds];
        NSArray *ids = [str componentsSeparatedByString:@","];
        
        [[ISN_GameCenterManager sharedInstance] sendAchievementChallenge:[ISNDataConvertor charToNSString:achievementId] message:[ISNDataConvertor charToNSString:message] playerIds:ids];
    }
    
    void _ISN_issueAchievementChallengeWithFriendsPicker(char* leaderboardId, char* message, char* playerIds) {
        
        NSString* lid = [ISNDataConvertor charToNSString:leaderboardId];
        NSString* mes = [ISNDataConvertor charToNSString:message];
        
        [[ISN_GameCenterManager sharedInstance] sendAchievementChallengeWithFriendsPicker:lid message:mes];
    }
    
    
    
    
    void _reportScore(char* score, char* leaderboardId) {
        
        NSString* lid = [ISNDataConvertor charToNSString:leaderboardId];
        NSString* scoreValue = [ISNDataConvertor charToNSString:score];
        
        [[ISN_GameCenterManager sharedInstance] reportScore:[scoreValue longLongValue] forCategory:lid];
    }
    
    void _submitAchievement(float percents, char* achievementID, BOOL notifyComplete) {
        double v = (double) percents;
        [[ISN_GameCenterManager sharedInstance] submitAchievement:v identifier:[ISNDataConvertor charToNSString:achievementID] notifyComplete:notifyComplete];
    }
    
    void _resetAchievements() {
        [[ISN_GameCenterManager sharedInstance] resetAchievements];
    }
    
    void _ISN_loadGKPlayerData(char* playerId) {
        [[ISN_GameCenterManager sharedInstance] loadPlayerInfoForPlayerWithId:[ISNDataConvertor charToNSString:playerId]];
    }
    
    void _ISN_loadGKPlayerPhoto(char* playerId, int size) {
        NSString* mPlayerId = [ISNDataConvertor charToNSString:playerId];
        [[ISN_GameCenterManager sharedInstance] loadImageForPlayerWithPlayerId:mPlayerId size:size];
    }
    
    void _ISN_RetrieveFriends() {
        [[ISN_GameCenterManager sharedInstance] retrieveFriends];
    }
    
    
    void _ISN_getSignature() {
        [[ISN_GameCenterManager sharedInstance] getSignature];
    }
    
    void _ISN_loadLeaderboardSetInfo() {
        [[ISN_GameCenterManager sharedInstance] loadLeaderboardSetInfo];
    }
    
    void _ISN_loadLeaderboardsForSet(char* setId) {
        NSString* lid = [ISNDataConvertor charToNSString:setId];
        [[ISN_GameCenterManager sharedInstance] loadLeaderboardsForSet:lid];
    }
    
}

