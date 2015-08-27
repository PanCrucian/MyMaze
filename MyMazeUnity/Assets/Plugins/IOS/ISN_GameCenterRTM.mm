
//  GCHelper.m
//  CatRace
//
//  Created by Ray Wenderlich on 4/23/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "ISN_GameCenterRTM.h"


NSString * const UNITY_SPLITTER = @"|";
NSString * const UNITY_SPLITTER2 = @"|%|";
//NSString * const UNITY_EOF = @"endofline";


@implementation ISN_GameCenterRTM


static GKMatch* currentMatch = nil;
static int playerGroup = 0;
static int playerAttributes = 0;
static BOOL isInited = FALSE;


@synthesize vc;

#pragma mark Initialization

static ISN_GameCenterRTM *sharedHelper = nil;
+ (ISN_GameCenterRTM *) sharedInstance {
    if (!sharedHelper) {
        sharedHelper = [[ISN_GameCenterRTM alloc] init];
        
    }
    return sharedHelper;
}


#pragma mark public

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
                UnitySendMessage("GameCenter_RTM", "OnMatchStartFailed", [ISNDataConvertor serializeError:error]);
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
            
            [[ISN_GameCenterManager sharedInstance] savePlayerInfo:player];
            
            NSMutableString * data = [[NSMutableString alloc] init];
            
            [data appendString:player.playerID];
            [data appendString: UNITY_SPLITTER];
            [data appendString: [NSString stringWithFormat:@"%d", response]];

            NSString *str = [data copy];
#if UNITY_VERSION < 500
            [str autorelease];
#endif
            NSLog(@"ISN RTM OnInviteeResponse");
            UnitySendMessage("GameCenterInvitations", "OnInviteeResponse", [ISNDataConvertor NSStringToChar:str]);
            
        };
        
    }
    
    NSLog(@"startFindMatchRequest useNativeUI %hhd", useNativeUI);
    
    if(useNativeUI) {
        
        GKMatchmakerViewController *mmvc = [[GKMatchmakerViewController alloc] initWithMatchRequest:request];
        mmvc.matchmakerDelegate = self;
        
        [self.vc presentViewController:mmvc animated:YES completion:nil];
        
        
    } else  {
        [[GKMatchmaker sharedMatchmaker] findMatchForRequest:request withCompletionHandler:^(GKMatch *match, NSError *error) {
            
            [self processMatchStartResult:match error:error];
            
        }];
    }
}

- (void) cancelMatchSeartch {
    [[GKMatchmaker sharedMatchmaker] cancel];
}

 -(void) initNotificationHandler {
     if(isInited) { return; }  isInited = TRUE;
     
     [GKMatchmaker sharedMatchmaker].inviteHandler = ^(GKInvite *acceptedInvite, NSArray *playersToInvite) {
     
         if (acceptedInvite) {
             
             UnitySendMessage("GameCenterInvitations", "OnPlayerAcceptedInvitation_RTM", [ISNDataConvertor NSStringToChar:[[ISN_GameCenterManager sharedInstance] serialiseInvite:acceptedInvite]]);
 
         } else {
             if(playersToInvite) {
                 
                 NSMutableArray* requestedInvitationsArray = [[NSMutableArray alloc] init];
                 for(GKPlayer* player in playersToInvite) {
                     [[ISN_GameCenterManager sharedInstance] savePlayerInfo:player];
                     [requestedInvitationsArray addObject:player.playerID];
                 }
                 
                  UnitySendMessage("GameCenterInvitations", "OnPlayerRequestedMatchWithRecipients_RTM", [ISNDataConvertor NSStringsArrayToChar:requestedInvitationsArray]);
             }
         }
         
     };
 }

-(void) startMatchWithInviteID:(NSString*) inviteId useNativeUI:(BOOL) useNativeUI {
    
    GKInvite* invite = [[ISN_GameCenterManager sharedInstance] getInviteWithId:inviteId];
    if(invite == nil) {
        NSLog(@"ISN startMatchWithInviteID, invite with id %@ not found", inviteId);
        return;
    }
    
    if(useNativeUI) {
        GKMatchmakerViewController *mmvc = [[GKMatchmakerViewController alloc] initWithInvite:invite];
        mmvc.matchmakerDelegate = self;
        
        [self.vc presentViewController:mmvc animated:YES completion:nil];
    } else {
        [[GKMatchmaker sharedMatchmaker] matchForInvite:invite completionHandler:^(GKMatch *match, NSError *error) {
            [self processMatchStartResult:match error:error];
        }];
    }
}




-(void) cancelPendingInviteToPlayerWithId:(NSString*) playerId {
    
    GKPlayer* player = [[ISN_GameCenterManager sharedInstance] getPlayerWithId:playerId];
    if(player == nil) {
         NSLog(@"ISN cancelPendingInviteToPlayerWithId, player with id %@ not found", playerId);
    }
    
    [[GKMatchmaker sharedMatchmaker] cancelPendingInviteToPlayer:player];
}

-(void) finishMatchmaking {
    if(currentMatch == nil) {
         NSLog(@"ISN finishMatchmaking, currentMatch is mill");
        return;
    }
    
    [[GKMatchmaker sharedMatchmaker] finishMatchmakingForMatch:currentMatch];
}

-(void) queryActivity {
    [[GKMatchmaker sharedMatchmaker] queryActivityWithCompletionHandler:^(NSInteger activity, NSError *error) {
        [self sendQueryActivityResult:activity error:error];
    }];
}

-(void) queryPlayerGroupActivity:(int) group {
    [[GKMatchmaker sharedMatchmaker] queryPlayerGroupActivity:group withCompletionHandler:^(NSInteger activity, NSError *error) {
        [self sendQueryActivityResult:activity error:error];
    }];
}


-(void) startBrowsingForNearbyPlayers {
    [[GKMatchmaker sharedMatchmaker] startBrowsingForNearbyPlayersWithHandler:^(GKPlayer *player, BOOL reachable) {
        
        NSMutableString * data = [[NSMutableString alloc] init];
        
        
        [data appendString:player.playerID];
        [data appendString: UNITY_SPLITTER];
        
        [data appendString: [NSString stringWithFormat:@"%d", reachable]];
        
        UnitySendMessage("GameCenter_RTM", "OnNearbyPlayerInfoReceived", [ISNDataConvertor NSStringToChar:data]);
        
    }];
}

-(void) stopBrowsingForNearbyPlayers {
    [[GKMatchmaker sharedMatchmaker] stopBrowsingForNearbyPlayers];
}


-(void) disconnect {
    if(currentMatch != nil) {
        [currentMatch disconnect];
    }
}

-(void) rematch {
    if(currentMatch != nil) {
        [currentMatch rematchWithCompletionHandler:^(GKMatch *match, NSError *error) {
            [self processMatchStartResult:match error:error];
        }];
    }
}

-(void)sendDataToAll:(NSData *)data withDataMode:(GKMatchSendDataMode)withDataMode {
    
    if(currentMatch == NULL) {
        return;
    }
    NSLog(@"sendDataToAll");
    
    NSError *error;
    BOOL IsdataWasSent =  [currentMatch sendDataToAllPlayers:data withDataMode:withDataMode error:&error];
    NSLog(@"IsdataWasSent: %hhd", IsdataWasSent);
    if (error != nil) {
        [self HandleDataSendError:error];
    }
}
    

- (void) sendData:(NSData *)data toPlayersWithIds:(NSArray *)toPlayersWithIds withDataMode:(GKMatchSendDataMode)withDataMode {
   
    if(currentMatch == NULL) {
        return;
    }
    
   /*
    
    NSMutableArray* players = [[NSMutableArray alloc] init];
    for (NSString* playerId : toPlayersWithIds) {
        GKPlayer* player = [[ISN_GameCenterManager sharedInstance] getPlayerWithId:playerId];
        [players addObject:player];
        
        NSLog(@"player added: %@", player.playerID);
    }
    */
    
    
    NSError *error = nil;
    BOOL IsdataWasSent =   [currentMatch sendData:data toPlayers:toPlayersWithIds withDataMode:withDataMode error:&error];
    NSLog(@"IsdataWasSent: %hhd", IsdataWasSent);
    if (error != nil) {
        NSLog(@"error.code: %d", error.code);
        NSLog(@"error.description: %@", error.description);
        
        [self HandleDataSendError:error];
    }

   
}


-(void) HandleDataSendError: (NSError *) error {
   // UnitySendMessage("GameCenter_RTM", "OnSendDataError", [ISNDataConvertor serializeError:error]);
}



#pragma private

-(void) sendQueryActivityResult:(NSInteger) activity error:(NSError *)error {
    
    if(error != nil) {
         UnitySendMessage("GameCenter_RTM", "OnQueryActivityFailed", [ISNDataConvertor serializeError:error]);
    } else {
        NSString *activityResult = [NSString stringWithFormat:@"%d", activity];
        UnitySendMessage("GameCenter_RTM", "OnQueryActivity", [ISNDataConvertor NSStringToChar:activityResult]);
    }
}

-(void) processMatchStartResult:(GKMatch *)match error:(NSError *) error {
    if(error != nil) {
        UnitySendMessage("GameCenter_RTM", "OnMatchStartFailed", [ISNDataConvertor serializeError:error]);
        return;
    }
    
    NSString* matchData = [self serializeMathcData:match];
    UnitySendMessage("GameCenter_RTM", "OnMatchStarted", [ISNDataConvertor NSStringToChar:matchData]);
}





-(void) updateCurrentMatch:(GKMatch *)match  {
    currentMatch = match;
    currentMatch.delegate = self;

}

-(NSString*) serializeMathcData:(GKMatch *)match  {
    NSLog(@"serialize match data");
    NSLog(@"match.players.count %d", match.players.count);
    NSLog(@"match.expectedPlayerCount %d", match.expectedPlayerCount);
    
    [self updateCurrentMatch:match];
    
    NSMutableString * data = [[NSMutableString alloc] init];

    
    [data appendString: [NSString stringWithFormat:@"%d", match.expectedPlayerCount]];
    [data appendString: UNITY_SPLITTER2];
    
    
    NSMutableArray* playersIds = [[NSMutableArray alloc] init];
    
    
    for(GKPlayer * player in match.players) {
        [[ISN_GameCenterManager sharedInstance] savePlayerInfo:player];
        [playersIds addObject:player.playerID];
    }
    
    [data appendString:[ISNDataConvertor serializeNSStringsArray:playersIds]];
    
    return data;
}


#pragma mark GKMatchDelegate

// The match received data sent from the player.
- (void)match:(GKMatch *)match didReceiveData:(NSData *)data fromRemotePlayer:(GKPlayer *)player {
    NSLog(@"RTM match didReceiveData");
    
    NSMutableString * str = [[NSMutableString alloc] init];
    
    [str appendString:player.playerID];
    [str appendString: UNITY_SPLITTER];
    [str appendString: [data base64Encoding]];
    
    
    NSString *info = [str copy] ;
    
    #if UNITY_VERSION < 500
        [info autorelease];
    #endif
    
     UnitySendMessage("GameCenter_RTM", "OnMatchDataReceived", [ISNDataConvertor NSStringToChar:info]);
    
}


// The player state changed (eg. connected or disconnected)
- (void)match:(GKMatch *)match player:(GKPlayer *)player didChangeConnectionState:(GKPlayerConnectionState)state  {
     NSLog(@"RTM player didChangeConnectionState");
    NSMutableString * str = [[NSMutableString alloc] init];
    
    [str appendString:player.playerID];
    [str appendString: UNITY_SPLITTER];
    [str appendString: [NSString stringWithFormat:@"%d", state]];
    
    
    NSString *info = [str copy] ;
    
#if UNITY_VERSION < 500
    [info autorelease];
#endif
    
    NSString* matchData = [self serializeMathcData:match];
    UnitySendMessage("GameCenter_RTM", "OnMatchInfoUpdated", [ISNDataConvertor NSStringToChar:matchData]);
    
    UnitySendMessage("GameCenter_RTM", "OnMatchPlayerStateChanged", [ISNDataConvertor NSStringToChar:info]);
}


// The match was unable to be established with any players due to an error.
- (void)match:(GKMatch *)match didFailWithError:(NSError *)error {
    NSLog(@"RTM match didFailWithError");
    UnitySendMessage("GameCenter_RTM", "OnMatchFailed", [ISNDataConvertor serializeError:error]);
}



// This method is called when the match is interrupted; if it returns YES, a new invite will be sent to attempt reconnection. This is supported only for 1v1 games
- (BOOL)match:(GKMatch *)match shouldReinviteDisconnectedPlayer:(GKPlayer *)player {
     NSLog(@"RTM match shouldReinviteDisconnectedPlayer");
    [[ISN_GameCenterManager sharedInstance] savePlayerInfo:player];
     UnitySendMessage("GameCenter_RTM", "OnDiconnectedPlayerReinvited", [ISNDataConvertor NSStringToChar:player.playerID]);
    return YES;
}



#pragma mark GKMatchmakerViewControllerDelegate


// The user has cancelled matchmaking
- (void)matchmakerViewControllerWasCancelled:(GKMatchmakerViewController *)viewController {
    NSLog(@"ISN: matchmakerViewControllerWasCancelled");
    
    [self.vc dismissViewControllerAnimated:YES completion:nil];
    UnitySendMessage("GameCenter_RTM", "OnMatchStartFailed", [ISNDataConvertor serializeErrorWithData:@"User Cancelled" code:0]);
}

// Matchmaking has failed with an error
- (void)matchmakerViewController:(GKMatchmakerViewController *)viewController didFailWithError:(NSError *)error  {
    NSLog(@"ISN: matchmakerViewControllerWasCancelled");
    
    [self.vc dismissViewControllerAnimated:YES completion:nil];
    UnitySendMessage("GameCenter_RTM", "OnMatchStartFailed", [ISNDataConvertor serializeError:error]);
}

- (void)matchmakerViewController:(GKMatchmakerViewController *)viewController didFindMatch:(GKMatch *)match  {
    NSLog(@"ISN: turnBasedMatchmakerViewController");
    [self.vc dismissViewControllerAnimated:YES completion:nil];
    
    NSString* matchData = [self serializeMathcData:match];
    UnitySendMessage("GameCenter_RTM", "OnMatchStarted", [ISNDataConvertor NSStringToChar:matchData]);

}

+(GKMatchSendDataMode) getDataModeByIntValue:(int) value {
    switch (value) {
        case 0:
            return GKMatchSendDataReliable;
            break;
            
        default:
            return GKMatchSendDataReliable;
            break;
    }
}

@end



extern "C" {
    
    
    void _ISN_RTM_FindMatch(int minPlayers, int maxPlayers, char* msg, char* invitations)  {
        
        NSString* inviteMessage = [ISNDataConvertor charToNSString:msg];
        NSArray* invitationsList = [ISNDataConvertor charToNSArray:invitations];
        
        [[ISN_GameCenterRTM sharedInstance] findMatch:minPlayers maxPlayers:maxPlayers inviteMessage:inviteMessage invitationsList:invitationsList];
    }
    
    void _ISN_RTM_FindMatchWithNativeUI(int minPlayers, int maxPlayers, char* msg, char* invitations)  {
        
        NSString* inviteMessage = [ISNDataConvertor charToNSString:msg];
        NSArray* invitationsList = [ISNDataConvertor charToNSArray:invitations];
        
        [[ISN_GameCenterRTM sharedInstance] findMatchWithNativeUI:minPlayers maxPlayers:maxPlayers inviteMessage:inviteMessage invitationsList:invitationsList];
    }

    
    void _ISN_RTM_SetPlayerGroup(int group)  {
        playerGroup = group;
    }
    
    void _ISN_RTM_SetPlayerAttributes(int attributes)  {
        playerAttributes = attributes;
    }
    
    void ISN_RTM_StartMatchWithInviteID(char* inviteId, BOOL useNativeUI) {
         NSString* m_inviteId = [ISNDataConvertor charToNSString:inviteId];
        
        [[ISN_GameCenterRTM sharedInstance] startMatchWithInviteID:m_inviteId useNativeUI:useNativeUI];
    }
    
    
    void ISN_RTM_CancelPendingInviteToPlayerWithId(char* playerId)  {
        NSString* m_playerId = [ISNDataConvertor charToNSString:playerId];
        
        [[ISN_GameCenterRTM sharedInstance] cancelPendingInviteToPlayerWithId:m_playerId];
    }
    
    void ISN_RTM_CancelMatchSeartch() {
        [[ISN_GameCenterRTM sharedInstance] cancelMatchSeartch];
    }
    
    void ISN_RTM_FinishMatchmaking () {
        [[ISN_GameCenterRTM sharedInstance] finishMatchmaking];
    }
    
    void ISN_RTM_QueryActivity () {
        [[ISN_GameCenterRTM sharedInstance] queryActivity];
    }
    
    
    void ISN_RTM_QueryPlayerGroupActivity(int group)  {
        [[ISN_GameCenterRTM sharedInstance] queryPlayerGroupActivity:group];
    }
    
    void ISN_RTM_StartBrowsingForNearbyPlayers() {
        [[ISN_GameCenterRTM sharedInstance] startBrowsingForNearbyPlayers];
    }
    
    
    void ISN_RTM_StopBrowsingForNearbyPlayers () {
        [[ISN_GameCenterRTM sharedInstance] stopBrowsingForNearbyPlayers];
    }
    
    void ISN_RTM_Rematch() {
        [[ISN_GameCenterRTM sharedInstance] rematch];
    }
    void ISN_RTM_Disconnect() {
        [[ISN_GameCenterRTM sharedInstance] disconnect];
    }
    
    void ISN_RTM_SendData(char* data, char* playersIds, int dataMode) {
        
        NSString* mDataString = [ISNDataConvertor charToNSString:data];
        NSData * s_data = [[NSData alloc] initWithBase64Encoding:mDataString];
        
        NSArray* s_playerIds =  [ISNDataConvertor charToNSArray:playersIds];
        
        
        GKMatchSendDataMode mode = [ISN_GameCenterRTM getDataModeByIntValue:dataMode];
        [[ISN_GameCenterRTM sharedInstance] sendData:s_data toPlayersWithIds:s_playerIds withDataMode:mode];
    }
    
    
    void ISN_RTM_SendDataToAll(char* data, int dataMode) {
        NSString* mDataString = [ISNDataConvertor charToNSString:data];
        NSData * s_data = [[NSData alloc] initWithBase64Encoding:mDataString];
        
        GKMatchSendDataMode mode = [ISN_GameCenterRTM getDataModeByIntValue:dataMode];
        
        [[ISN_GameCenterRTM sharedInstance] sendDataToAll:s_data withDataMode:mode];
    }

    
    
}
