#import "ISN_GameCenterTBM.h"


NSString * const UNITY_SPLITTER = @"|";
NSString * const UNITY_SPLITTER2 = @"|%|";
NSString * const UNITY_EOF = @"endofline";

@implementation ISN_GameCenterTBM


static ISN_GameCenterTBM *_sharedInstance;
static NSMutableArray *matches;

static int playerGroup = 0;
static int playerAttributes = 0;

@synthesize vc;

+ (id)sharedInstance {
    
    if (_sharedInstance == nil)  {
        matches  = [[NSMutableArray alloc] init];
        _sharedInstance = [[self alloc] init];
 
    }
    
    return _sharedInstance;
}



-(void) findMatch:(int)minPlayers maxPlayers:(int)maxPlayers inviteMessage:(NSString *)inviteMessage invitationsList:(NSArray *)invitationsList {
    [self prepareforMacthRequest:minPlayers maxPlayers:maxPlayers inviteMessage:inviteMessage invitationsList:invitationsList useNativeUI:false];
}

- (void)findMatchWithNativeUI:(int)minPlayers maxPlayers:(int)maxPlayers inviteMessage:(NSString *)inviteMessage invitationsList:(NSArray *)invitationsList{
    [self prepareforMacthRequest:minPlayers maxPlayers:maxPlayers inviteMessage:inviteMessage invitationsList:invitationsList useNativeUI:true];
}

-(void) prepareforMacthRequest:(int)minPlayers maxPlayers:(int)maxPlayers inviteMessage:(NSString *)inviteMessage invitationsList:(NSArray *)invitationsList useNativeUI:(BOOL) useNativeUI {
    
    if(invitationsList.count > 0) {
        [GKPlayer loadPlayersForIdentifiers:invitationsList withCompletionHandler:^(NSArray *players, NSError *error) {
            if(error == nil) {
                [self startFindMatchRequest:minPlayers maxPlayers:maxPlayers inviteMessage:inviteMessage recipients:players useNativeUI:useNativeUI];
            } else {
                UnitySendMessage("GameCenter_TBM", "OnMatchFoundResultFailed", [ISNDataConvertor serializeError:error]);
            }
        }];
    } else {
        [self startFindMatchRequest:minPlayers maxPlayers:maxPlayers inviteMessage:inviteMessage recipients:invitationsList useNativeUI:useNativeUI];
    }
}

-(void) startFindMatchRequest:(int)minPlayers maxPlayers:(int)maxPlayers inviteMessage:(NSString *)inviteMessage recipients:(NSArray *)recipients useNativeUI:(BOOL) useNativeUI  {

    GKMatchRequest *request = [[GKMatchRequest alloc] init];
    request.minPlayers = minPlayers;
    request.maxPlayers = maxPlayers;
    
    if(playerGroup != 0) {
        request.playerGroup = playerGroup;
    }
    
    if(playerAttributes != 0) {
        request.playerAttributes = playerAttributes;
    }
    
    if(![inviteMessage isEqualToString:@""]) {
        request.inviteMessage = inviteMessage;
    }
    
    if(recipients.count > 0) {
        request.recipients = recipients;
        request.recipientResponseHandler = ^( GKPlayer *player, GKInviteeResponse response) {
            
            NSMutableString * data = [[NSMutableString alloc] init];
            
            [data appendString:player.playerID];
            [data appendString: UNITY_SPLITTER];
            [data appendString: [NSString stringWithFormat:@"%d", response]];
            
            NSLog(@"ISN RTM OnInviteeResponse");
            UnitySendMessage("GameCenterInvitations", "OnInviteeResponse", [ISNDataConvertor NSStringToChar:data]);
            
        };

    }
    
    NSLog(@"ISN startFindMatchRequest useNativeUI %hhd", useNativeUI);
    
    if(useNativeUI) {
        
        GKTurnBasedMatchmakerViewController *mmvc = [[GKTurnBasedMatchmakerViewController alloc] initWithMatchRequest:request];
        mmvc.turnBasedMatchmakerDelegate = self;
        [self.vc presentViewController:mmvc animated:YES completion:nil];
        
    } else  {
        [GKTurnBasedMatch findMatchForRequest: request withCompletionHandler:^(GKTurnBasedMatch *match, NSError *error) {
            if (error == NULL) {
                NSString* matchData = [self serializeMathcData:match];
                [self updateMatchInfo:match];
                UnitySendMessage("GameCenter_TBM", "OnMatchFoundResult", [ISNDataConvertor NSStringToChar:matchData]);
            } else {
                UnitySendMessage("GameCenter_TBM", "OnMatchFoundResultFailed", [ISNDataConvertor serializeError:error]);
            }
        }];
    }
    
}

-(void) startMatchWithInviteID:(NSString*) inviteId useNativeUI:(BOOL) useNativeUI {
    
    GKInvite* invite = [[ISN_GameCenterManager sharedInstance] getInviteWithId:inviteId];
    if(invite == nil) {
        NSLog(@"ISN startMatchWithInviteID, invite with id %@ not found", inviteId);
        return;
    }
   
}

-(void) loadMatches {
    [GKTurnBasedMatch loadMatchesWithCompletionHandler:^(NSArray *matches, NSError *error) {
        if(error == nil) {
            if (matches) {
                NSLog(@"Loaded matches count %lu", (unsigned long)matches.count);
                
                
                NSMutableString * data = [[NSMutableString alloc] init];
                
                
                for(GKTurnBasedMatch * m in matches){
                    [data appendString: [self serializeMathcData:m]];
                    [data appendString: UNITY_SPLITTER2];
                    [self updateMatchInfo:m];
                }
                
                [data appendString: UNITY_EOF];
                
                NSString *str = [data copy];
#if UNITY_VERSION < 500
                [str autorelease];
#endif
                
                UnitySendMessage("GameCenter_TBM", "OnLoadMatchesResult", [ISNDataConvertor NSStringToChar:data]);
            } else {
                UnitySendMessage("GameCenter_TBM", "OnLoadMatchesResult", [ISNDataConvertor NSStringToChar:@""]);
            }
        } else {
            
            
            UnitySendMessage("GameCenter_TBM", "OnLoadMatchesResultFailed", [ISNDataConvertor serializeError:error]);
        }
        
    }];
}


-(void) loadMatch:(NSString *)matchId {
    [GKTurnBasedMatch loadMatchWithID:matchId withCompletionHandler:^(GKTurnBasedMatch *match, NSError *error) {
        if(error == nil) {
            [self updateMatchInfo:match];
            NSString* matchData = [self serializeMathcData:match];
            UnitySendMessage("GameCenter_TBM", "OnLoadMatchResult", [ISNDataConvertor NSStringToChar:matchData]);
        } else {
            UnitySendMessage("GameCenter_TBM", "OnLoadMatchResultFailed",  [ISNDataConvertor serializeError:error]);
        }
    }];
}


-(void) saveCurrentTurn:(NSString *)matchId updatedMatchData:(NSData *)updatedMatchData {
    NSLog(@"ISN saveCurrentTurn");
    GKTurnBasedMatch* match = [self getMatchWithId:matchId];
    if(match != NULL) {
        [match saveCurrentTurnWithMatchData:updatedMatchData completionHandler:^(NSError *error) {
            if(error != nil) {
                UnitySendMessage("GameCenter_TBM", "OnUpdateMatchResultFailed", [ISNDataConvertor serializeError:error]);
            } else {
                NSString *encodedData = [updatedMatchData base64Encoding];
                
                NSMutableString * data = [[NSMutableString alloc] init];
                
                
                [data appendString: matchId];
                [data appendString: UNITY_SPLITTER];
                [data appendString: encodedData];
              
                NSString *str = [data copy];
                
                #if UNITY_VERSION < 500
                    [str autorelease];
                #endif

                
                
                UnitySendMessage("GameCenter_TBM", "OnUpdateMatchResult", [ISNDataConvertor NSStringToChar:str]);
            }
        }];
    } else {
       UnitySendMessage("GameCenter_TBM", "OnUpdateMatchResultFailed", [ISNDataConvertor serializeErrorWithData:@"Match Not Found" code:0]);
    }
}

-(void) quitInTurn:(NSString *)matchId outcome:(int)outcome nextPlayerId:(NSString*)nextPlayerId matchData:(NSData *)matchData {
    NSLog(@"ISN quitInTurn");
    GKTurnBasedMatch* match = [self getMatchWithId:matchId];
    if(match != NULL) {
    
 
        GKTurnBasedMatchOutcome participantOutcome = [ISN_GameCenterTBM getOutcomeByIntValue:outcome];
        GKTurnBasedParticipant *nextParticipantObject = [self getMathcParticipantById:match playerId:nextPlayerId];
        
       [match participantQuitInTurnWithOutcome:participantOutcome nextParticipant:nextParticipantObject matchData:matchData completionHandler:^(NSError *error) {
           if(error == nil) {
               UnitySendMessage("GameCenter_TBM", "OnMatchQuitResult", [ISNDataConvertor NSStringToChar:matchId]);
           } else {
               UnitySendMessage("GameCenter_TBM", "OnMatchQuitResultFailed", [ISNDataConvertor serializeError:error]);
           }
       }];
    
    } else {
        UnitySendMessage("GameCenter_TBM", "OnMatchQuitResultFailed", [ISNDataConvertor serializeErrorWithData:@"Match Not Found" code:0]);
    }
}

- (void) quitOutOfTurn:(NSString *)matchId outcome:(int)outcome {
    NSLog(@"ISN quitOutOfTurn");
    GKTurnBasedMatch* match = [self getMatchWithId:matchId];
    if(match != NULL) {
        
        GKTurnBasedMatchOutcome participantOutcome = [ISN_GameCenterTBM getOutcomeByIntValue:outcome];
        [match participantQuitOutOfTurnWithOutcome:participantOutcome withCompletionHandler:^(NSError *error) {
            if(error == nil) {
                 UnitySendMessage("GameCenter_TBM", "OnMatchQuitResult", [ISNDataConvertor NSStringToChar:matchId]);
            } else {
                 UnitySendMessage("GameCenter_TBM", "OnMatchQuitResultFailed", [ISNDataConvertor serializeError:error]);
            }
        }];
        
    } else {
        UnitySendMessage("GameCenter_TBM", "OnMatchQuitResultFailed", [ISNDataConvertor serializeErrorWithData:@"Match Not Found" code:0]);
    }
}


-(void) updatePlayerOutcome:(NSString *)matchId Outcome:(int)Outcome playerId:(NSString*)playerId {
    NSLog(@"ISN updatePlayerOutcome");
    GKTurnBasedMatch* match = [self getMatchWithId:matchId];
    GKTurnBasedMatchOutcome participantOutcome = [ISN_GameCenterTBM getOutcomeByIntValue:Outcome];
    if(match != NULL) {
        
        GKTurnBasedParticipant *participant =  [self getMathcParticipantById:match playerId:playerId];
        if(participant != nil) {
            [participant setMatchOutcome:participantOutcome];
        } else {
            NSLog(@"ISN::updatePlayerOutcome participant not found");
        }
    } else {
        NSLog(@"ISN::updatePlayerOutcome match not found");
    }
}


-(void) endTurn:(NSString *)matchId updatedMatchData:(NSData *)updatedMatchData nextPlayerId:(NSString*)nextPlayerId {
    NSLog(@"ISN endTurn with next player");
    GKTurnBasedMatch* match = [self getMatchWithId:matchId];
    if(match != NULL) {
        GKTurnBasedParticipant *nextParticipant = [self getMathcParticipantById:match playerId:nextPlayerId];
        
        [match endTurnWithNextParticipant:nextParticipant matchData:updatedMatchData completionHandler:^(NSError *error) {
            [self handleCompleteTurnResult:error requestedMatch:match];
        }];
        
    } else {
        UnitySendMessage("GameCenter_TBM", "OnEndTurnResultFailed", [ISNDataConvertor serializeErrorWithData:@"Match Not Found" code:0]);
    }
}


-(void) handleCompleteTurnResult: (NSError* )error requestedMatch: (GKTurnBasedMatch*)requestedMatch {
    if(error != nil) {
        UnitySendMessage("GameCenter_TBM", "OnEndTurnResultFailed", [ISNDataConvertor serializeError: error]);
        return;
    }
    
    [GKTurnBasedMatch loadMatchWithID:requestedMatch.matchID withCompletionHandler:^(GKTurnBasedMatch *match, NSError *error) {
        if(error != nil) {
            UnitySendMessage("GameCenter_TBM", "OnEndTurnResultFailed", [ISNDataConvertor serializeError: error]);
        } else {
            NSString* matchData = [self serializeMathcData:match];
            UnitySendMessage("GameCenter_TBM", "OnEndTurnResult", [ISNDataConvertor NSStringToChar:matchData]);
        }
    }];
}


-(void) endMatch:(NSString *)matchId matchData:(NSData *)matchData {
     NSLog(@"ISN endMatch");
    GKTurnBasedMatch* match = [self getMatchWithId:matchId];
    if(match != NULL) {
        [match endMatchInTurnWithMatchData:matchData completionHandler:^(NSError *error) {
            if(error == nil) {
                NSString* matchData = [self serializeMathcData:match];
                UnitySendMessage("GameCenter_TBM", "OnEndMatch", [ISNDataConvertor NSStringToChar:matchData]);
            } else {
                UnitySendMessage("GameCenter_TBM", "OnEndMatchResult", [ISNDataConvertor serializeError: error]);
            }
        }];
    } else {
         UnitySendMessage("GameCenter_TBM", "OnEndMatchResult", [ISNDataConvertor serializeErrorWithData:@"Match Not Found" code:0]);
    }
}


-(void) rematch:(NSString *)matchId {
     NSLog(@"ISN rematch");
    GKTurnBasedMatch* match = [self getMatchWithId:matchId];
    if(match != NULL) {
        [match rematchWithCompletionHandler:^(GKTurnBasedMatch *match, NSError *error) {
            if (error == NULL) {
                [self updateMatchInfo:match];
                NSString* matchData = [self serializeMathcData:match];
                UnitySendMessage("GameCenter_TBM", "OnRematchResult", [ISNDataConvertor NSStringToChar:matchData]);
            } else {
                UnitySendMessage("GameCenter_TBM", "OnRematchFailed", [ISNDataConvertor serializeError:error]);
            }
        }];
    } else {
        UnitySendMessage("GameCenter_TBM", "OnRematchFailed", [ISNDataConvertor serializeErrorWithData:@"Match Not Found" code:0]);
    }
    
}

-(void) removeMatch:(NSString *)matchId {
    NSLog(@"ISN removeMatch");
    GKTurnBasedMatch* match = [self getMatchWithId:matchId];
    if(match != NULL) {
        [match removeWithCompletionHandler:^(NSError *error) {
            if(error == nil) {
                 UnitySendMessage("GameCenter_TBM", "OnMatchRemoved", [ISNDataConvertor NSStringToChar:matchId]);
            } else {
                UnitySendMessage("GameCenter_TBM", "OnMatchRemoveFailed", [ISNDataConvertor serializeError:error]);
            }
        }];
    } else {
        UnitySendMessage("GameCenter_TBM", "OnMatchRemoveFailed", [ISNDataConvertor serializeErrorWithData:@"Match Not Found" code:0]);
    }
}



#pragma private



-(GKTurnBasedParticipant *) getMathcParticipantById: (GKTurnBasedMatch*) match playerId: (NSString*)playerId {
    
    NSArray *participants = match.participants;
     for(GKTurnBasedParticipant * p in participants){
         if(p.player == nil) {
             if(playerId.length == 0) {
                 return  p;
             }
         } else {
             if([p.playerID isEqualToString:playerId]) {
                 return p;
             }
         }
     }
    
    return  nil;
}





-(void) updateMatchInfo:(GKTurnBasedMatch *)match {
    
    BOOL macthFound = FALSE;
    int replaceIndex = 0;
    int currentIndex = 0;
    for(GKTurnBasedMatch * m in matches){
        
        
        if([m.matchID isEqualToString:match.matchID]) {
            macthFound = TRUE;
            replaceIndex = currentIndex;
            break;
        }
    
        currentIndex++;
    }
    
    if(macthFound) {
        [matches replaceObjectAtIndex:replaceIndex withObject:match];
    } else {
        [matches addObject:match];
    }
    
     //TODO: send match state update event
}

-(GKTurnBasedMatch *) getMatchWithId:(NSString*) matchId {
     for(GKTurnBasedMatch * m in matches){
         if([m.matchID isEqualToString:matchId]) {
             return m;
         }
     }
    
    return NULL;
}

#pragma Serizlization




-(NSString*) serializeMathcData:(GKTurnBasedMatch *)match  {
    
    
    NSMutableString * data = [[NSMutableString alloc] init];
    
   
    [data appendString: match.matchID];
    [data appendString: UNITY_SPLITTER];
    
    [data appendString: [NSString stringWithFormat:@"%d", match.status]];
    [data appendString: UNITY_SPLITTER];
    
    
    if(match.message != nil) {
        [data appendString: match.message];
    } else {
        [data appendString:@""];
    }

    [data appendString: UNITY_SPLITTER];
    
    
    NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
#if UNITY_VERSION < 500
    [dateFormatter autorelease];
#endif
    
    [dateFormatter setDateFormat: @"yyyy-MM-dd HH:mm:ss"];
    NSString *creationDateString = [dateFormatter stringFromDate:match.creationDate];
    [data appendString: creationDateString];
    [data appendString: UNITY_SPLITTER];
    
    if(match.matchData != nil) {
        [data appendString: [match.matchData base64Encoding]];
    } else {
        [data appendString: @""];
    }
    
    [data appendString: UNITY_SPLITTER];
    
    if(match.currentParticipant.player != nil) {
        [data appendString:match.currentParticipant.player.playerID];
    } else {
        [data appendString: @""];
    }
    
    [data appendString: UNITY_SPLITTER];
    
    
    
    [data appendString:[self serializeParticipantsInfo:match.participants]];
    
    NSString *str = [data copy];
    #if UNITY_VERSION < 500
        [str autorelease];
    #endif
    
    return str;
    
}

-(NSString*) serializeParticipantsInfo :(NSArray*)participants {
    NSMutableString * data = [[NSMutableString alloc] init];
    
    for(GKTurnBasedParticipant * p in participants){
        [data appendString:[self serializeParticipantInfo:p]];
        [data appendString: UNITY_SPLITTER];
    }
    
    [data appendString: UNITY_EOF];
    
    
    NSString *str = [data copy];
    #if UNITY_VERSION < 500
        [str autorelease];
    #endif
    
    return str;
}

-(NSString*) serializeParticipantInfo :(GKTurnBasedParticipant *) participant {
     NSMutableString * data = [[NSMutableString alloc] init];
    
    
    if(participant.player != nil) {
        NSLog(@"Parsing participant ith id: %@",participant.player.playerID);
        [[ISN_GameCenterManager sharedInstance] savePlayerInfo:participant.player];
        [data appendString: participant.player.playerID];
    } else {
         NSLog(@"Parsing participant ith id none ");
        [data appendString:@""];
    }
    [data appendString: UNITY_SPLITTER];
    

    [data appendString: [NSString stringWithFormat:@"%d", participant.status]];
    [data appendString: UNITY_SPLITTER];
    [data appendString: [NSString stringWithFormat:@"%d", participant.matchOutcome]];
    [data appendString: UNITY_SPLITTER];
    
    NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
    #if UNITY_VERSION < 500
        [dateFormatter autorelease];
    #endif
    
    
    
    [dateFormatter setDateFormat: @"yyyy-MM-dd HH:mm:ss"];
    
    if(participant.timeoutDate != nil) {
        NSString *timeoutDateString = [dateFormatter stringFromDate:participant.timeoutDate];
        [data appendString: timeoutDateString];
    } else {
        [data appendString: @"1970-01-01 00:00:00"];
    }
    [data appendString: UNITY_SPLITTER];
    
    
    
    if(participant.lastTurnDate != nil) {
        NSString *lastTurnDateString = [dateFormatter stringFromDate:participant.lastTurnDate];
        [data appendString: lastTurnDateString];
    } else {
        [data appendString: @"1970-01-01 00:00:00"];
    }
 
    
    NSString *str = [data copy];
    #if UNITY_VERSION < 500
        [str autorelease];
    #endif
    
    return str;
}

#pragma TMB Delegate methods

-(void) turnBasedMatchmakerViewControllerWasCancelled:(GKTurnBasedMatchmakerViewController *)viewController {
    NSLog(@"ISN: turnBasedMatchmakerViewControllerWasCancelled");
  
    [self.vc dismissViewControllerAnimated:YES completion:nil];
    UnitySendMessage("GameCenter_TBM", "OnMatchFoundResultFailed", [ISNDataConvertor serializeErrorWithData:@"User Cancelled" code:0]);
}

-(void) turnBasedMatchmakerViewController:(GKTurnBasedMatchmakerViewController *)viewController didFailWithError:(NSError *)error {
    NSLog(@"ISN: turnBasedMatchmakerViewController");
    [self.vc dismissViewControllerAnimated:YES completion:nil];
    UnitySendMessage("GameCenter_TBM", "OnMatchFoundResultFailed", [ISNDataConvertor serializeError:error]);
}

-(void) turnBasedMatchmakerViewController:(GKTurnBasedMatchmakerViewController *)viewController playerQuitForMatch:(GKTurnBasedMatch *)match {
    NSLog(@"ISN: turnBasedMatchmakerViewController");
    [self.vc dismissViewControllerAnimated:YES completion:nil];
    
    NSString* matchData = [self serializeMathcData:match];
    UnitySendMessage("GameCenter_TBM", "OnPlayerQuitForMatch", [ISNDataConvertor NSStringToChar:matchData]);

}

-(void) turnBasedMatchmakerViewController:(GKTurnBasedMatchmakerViewController *)viewController didFindMatch:(GKTurnBasedMatch *)match {
    NSLog(@"ISN: turnBasedMatchmakerViewController");
    [self.vc dismissViewControllerAnimated:YES completion:nil];
    
    NSString* matchData = [self serializeMathcData:match];
    UnitySendMessage("GameCenter_TBM", "OnMatchFoundResult", [ISNDataConvertor NSStringToChar:matchData]);
}


+(GKTurnBasedMatchOutcome) getOutcomeByIntValue:(int) value {
    switch (value) {
        case 0:
            return GKTurnBasedMatchOutcomeNone;
        case 1:
            return GKTurnBasedMatchOutcomeQuit;
        case 2:
            return GKTurnBasedMatchOutcomeWon;
        case 3:
            return GKTurnBasedMatchOutcomeLost;
        case 4:
            return GKTurnBasedMatchOutcomeTied;
        case 5:
            return GKTurnBasedMatchOutcomeTimeExpired;
        case 6:
            return GKTurnBasedMatchOutcomeFirst;
        case 7:
            return GKTurnBasedMatchOutcomeSecond;
        case 8:
            return GKTurnBasedMatchOutcomeThird;
        case 9:
            return GKTurnBasedMatchOutcomeFourth;
            
        default:
            return GKTurnBasedMatchOutcomeNone;
            break;
    }
}



@end





extern "C" {

    
    
    
    void _ISN_TBM_LoadMatchesInfo()  {
        [[ISN_GameCenterTBM sharedInstance] loadMatches];
    }
    
    
    void _ISN_TBM_LoadMatch(char* mId)  {
        NSString* matchId = [ISNDataConvertor charToNSString:mId];
        [[ISN_GameCenterTBM sharedInstance] loadMatch:matchId];
    }

    
    void _ISN_TBM_FindMatch(int minPlayers, int maxPlayers, char* msg, char* invitations)  {
        NSString* inviteMessage = [ISNDataConvertor charToNSString:msg];
        NSArray* invitationsList = [ISNDataConvertor charToNSArray:invitations];
        
        [[ISN_GameCenterTBM sharedInstance] findMatch:minPlayers maxPlayers:maxPlayers inviteMessage:inviteMessage invitationsList:invitationsList];
    }
    
    void _ISN_TBM_FindMatchWithNativeUI(int minPlayers, int maxPlayers, char* msg, char* invitations)  {
        NSString* inviteMessage = [ISNDataConvertor charToNSString:msg];
        NSArray* invitationsList = [ISNDataConvertor charToNSArray:invitations];
        
        [[ISN_GameCenterTBM sharedInstance] findMatchWithNativeUI:minPlayers maxPlayers:maxPlayers inviteMessage:inviteMessage invitationsList:invitationsList];
    }
    
    void _ISN_TBM_SetPlayerGroup(int group)  {
        playerGroup = group;
    }
    
    void _ISN_TBM_SetPlayerAttributes(int attributes)  {
        playerAttributes = attributes;
    }
    
    void _ISN_TBM_SaveCurrentTurn(char* matchId, char* data)  {
        NSString* mId = [ISNDataConvertor charToNSString:matchId];
        
        NSString* mDataString = [ISNDataConvertor charToNSString:data];
        NSData *mData = [[NSData alloc] initWithBase64Encoding:mDataString];
        
        [[ISN_GameCenterTBM sharedInstance] saveCurrentTurn:mId updatedMatchData:mData];
    }
    
   
    void _ISN_TBM_EndTurn(char* matchId, char* data, char* nextPlayerId)  {
        NSString* mId = [ISNDataConvertor charToNSString:matchId];
        
        NSString* mDataString = [ISNDataConvertor charToNSString:data];
        NSData *mData = [[NSData alloc] initWithBase64Encoding:mDataString];
        NSString* mNextPlayerId = [ISNDataConvertor charToNSString:nextPlayerId];
       
        [[ISN_GameCenterTBM sharedInstance] endTurn:mId updatedMatchData:mData nextPlayerId:mNextPlayerId];

    }
    
    void _ISN_TBM_QuitInTurn(char* matchId, int outcome, char* nextPlayerId, char* data) {
        NSString* mId = [ISNDataConvertor charToNSString:matchId];
        
        NSString* mDataString = [ISNDataConvertor charToNSString:data];
        NSData *mData = [[NSData alloc] initWithBase64Encoding:mDataString];
        NSString* mNextPlayerId = [ISNDataConvertor charToNSString:nextPlayerId];
        
        
        
        
        [[ISN_GameCenterTBM sharedInstance] quitInTurn:mId outcome:outcome nextPlayerId:mNextPlayerId matchData:mData];
    }
    
    void _ISN_TBM_QuitOutOfTurn(char* matchId, int outcome) {
        NSString* mId = [ISNDataConvertor charToNSString:matchId];
        [[ISN_GameCenterTBM sharedInstance] quitOutOfTurn:mId outcome:outcome];
    }
    
    void _ISN_TBM_UpdateParticipantOutcome(char* matchId, int outcome, char* playerId) {
        NSString* mId = [ISNDataConvertor charToNSString:matchId];
        NSString* mPlayerId = [ISNDataConvertor charToNSString:playerId];
        
        [[ISN_GameCenterTBM sharedInstance] updatePlayerOutcome:mId Outcome:outcome playerId:mPlayerId];
    }

    
    void _ISN_TBM_EndMatch(char* matchId, char* data)  {
        NSString* mId = [ISNDataConvertor charToNSString:matchId];
        
        NSString* mDataString = [ISNDataConvertor charToNSString:data];
        NSData *mData = [[NSData alloc] initWithBase64Encoding:mDataString];
        
        [[ISN_GameCenterTBM sharedInstance] endMatch:mId matchData:mData];
    }
    
    void _ISN_TBM_Rematch(char* matchId)  {
        NSString* mId = [ISNDataConvertor charToNSString:matchId];
        
        [[ISN_GameCenterTBM sharedInstance] rematch:mId];
    }
    
    void _ISN_TBM_RemoveMatch(char* matchId)  {
        NSString* mId = [ISNDataConvertor charToNSString:matchId];
        [[ISN_GameCenterTBM sharedInstance] removeMatch:mId];
    }
    
    
}








