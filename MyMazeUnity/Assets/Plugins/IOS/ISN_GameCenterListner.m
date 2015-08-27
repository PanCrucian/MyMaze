
//  GCHelper.m
//  CatRace
//
//  Created by Ray Wenderlich on 4/23/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "ISN_GameCenterListner.h"
#import "ISN_NativeUtility.h"

@implementation ISN_GameCenterListner


#pragma mark Initialization

static ISN_GameCenterListner *sharedHelper = nil;
+ (ISN_GameCenterListner *) sharedInstance {
    if (!sharedHelper) {
        sharedHelper = [[ISN_GameCenterListner alloc] init];
        
    }
    return sharedHelper;
}



- (id)init {
    if ((self = [super init])) {
        isInited = false; 
    }
    
    return self;
}

- (void) subscribe {
    
    if([ISN_NativeUtility majorIOSVersion] >= 8) {
        GKLocalPlayer *localPlayer = [GKLocalPlayer localPlayer];
        [localPlayer registerListener:self];
        NSLog(@"subscribed");

    }
}

#pragma GKInviteEventListener


// player:didAcceptInvite: gets called when another player accepts the invite from the local player
- (void)player:(GKPlayer *)player didAcceptInvite:(GKInvite *)invite  {
      NSLog(@"ISN GKInviteEventListener::didAcceptInvite");
     // UnitySendMessage("GameCenterInvitations", "OnPlayerAcceptedInvitation_TBM", [ISNDataConvertor NSStringToChar:[[ISN_GameCenterManager sharedInstance] serialiseInvite:invite]]);
}

// didRequestMatchWithRecipients: gets called when the player chooses to play with another player from Game Center and it launches the game to start matchmaking
- (void)player:(GKPlayer *)player didRequestMatchWithRecipients:(NSArray *)recipientPlayers  {
   
     NSLog(@"ISN GKInviteEventListener::didRequestMatchWithRecipients");
    
    NSMutableArray* requestedInvitationsArray = [[NSMutableArray alloc] init];
    for(GKPlayer* player in recipientPlayers) {
        [[ISN_GameCenterManager sharedInstance] savePlayerInfo:player];
        [requestedInvitationsArray addObject:player.playerID];
    }
    
    UnitySendMessage("GameCenterInvitations", "OnPlayerRequestedMatchWithRecipients_TBM", [ISNDataConvertor NSStringsArrayToChar:requestedInvitationsArray]);

}



#pragma GKTurnBasedEventListener


// If Game Center initiates a match the developer should create a GKTurnBasedMatch from playersToInvite and present a GKTurnbasedMatchmakerViewController.
- (void)player:(GKPlayer *)player didRequestMatchWithOtherPlayers:(NSArray *)playersToInvite {
    
    NSLog(@"ISN GKTurnBasedEventListener::didRequestMatchWithOtherPlayers");
    
    NSMutableArray* requestedInvitationsArray = [[NSMutableArray alloc] init];
    for(GKPlayer* player in playersToInvite) {
        [[ISN_GameCenterManager sharedInstance] savePlayerInfo:player];
        [requestedInvitationsArray addObject:player.playerID];
    }
    
    UnitySendMessage("GameCenterInvitations", "OnPlayerRequestedMatchWithRecipients_TBM", [ISNDataConvertor NSStringsArrayToChar:requestedInvitationsArray]);

}

// called when it becomes this player's turn.  It also gets called under the following conditions:
//      the player's turn has a timeout and it is about to expire.
//      the player accepts an invite from another player.
// when the game is running it will additionally recieve turn events for the following:
//      turn was passed to another player
//      another player saved the match data
// Because of this the app needs to be prepared to handle this even while the player is taking a turn in an existing match.  The boolean indicates whether this event launched or brought to forground the app.


- (void)player:(GKPlayer *)player receivedTurnEventForMatch:(GKTurnBasedMatch *)match didBecomeActive:(BOOL)didBecomeActive {
     NSLog(@"ISN GKTurnBasedEventListener::receivedTurnEventForMatch");
    
    [[ISN_GameCenterTBM sharedInstance] updateMatchInfo:match];
    NSString* matchData = [[ISN_GameCenterTBM sharedInstance] serializeMathcData:match];
    UnitySendMessage("GameCenter_TBM", "OnTrunReceived", [ISNDataConvertor NSStringToChar:matchData]);
    
}

// called when the match has ended.
- (void)player:(GKPlayer *)player matchEnded:(GKTurnBasedMatch *)match {
     NSLog(@"ISN GKTurnBasedEventListener::matchEnded");
    
    [[ISN_GameCenterTBM sharedInstance] updateMatchInfo:match];
    
    NSString* matchData = [[ISN_GameCenterTBM sharedInstance] serializeMathcData:match];
    UnitySendMessage("GameCenter_TBM", "OnEndMatch", [ISNDataConvertor NSStringToChar:matchData]);
}





// this is called when a player receives an exchange request from another player.
- (void)player:(GKPlayer *)player receivedExchangeRequest:(GKTurnBasedExchange *)exchange forMatch:(GKTurnBasedMatch *)match {
     NSLog(@"ISN GKTurnBasedEventListener::receivedExchangeRequest");
}

// this is called when an exchange is canceled by the sender.
- (void)player:(GKPlayer *)player receivedExchangeCancellation:(GKTurnBasedExchange *)exchange forMatch:(GKTurnBasedMatch *)match  {
     NSLog(@"ISN GKTurnBasedEventListener::receivedExchangeCancellation");
}

// called when all players either respond or timeout responding to this request.  This is sent to both the turn holder and the initiator of the exchange
- (void)player:(GKPlayer *)player receivedExchangeReplies:(NSArray *)replies forCompletedExchange:(GKTurnBasedExchange *)exchange forMatch:(GKTurnBasedMatch *)match {
      NSLog(@"ISN GKTurnBasedEventListener::receivedExchangeReplies");
}



@end
