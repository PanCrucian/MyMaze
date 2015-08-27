//
//  GCHelper.h
//  CatRace
//
//  Created by Ray Wenderlich on 4/23/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <GameKit/GameKit.h>
#import "ISN_GameCenterManager.h"



@interface ISN_GameCenterRTM : NSObject <GKMatchmakerViewControllerDelegate, GKMatchDelegate>


@property (nonatomic, retain) UIViewController *vc;

+ (ISN_GameCenterRTM *)sharedInstance;

- (void) initNotificationHandler;

-(void) findMatch:(int)minPlayers maxPlayers:(int) maxPlayers inviteMessage:(NSString*) inviteMessage invitationsList:(NSArray*) invitationsList;
-(void) findMatchWithNativeUI:(int)minPlayers maxPlayers:(int) maxPlayers inviteMessage:(NSString*) inviteMessage invitationsList:(NSArray*) invitationsList;
-(void) startMatchWithInviteID:(NSString*) inviteId useNativeUI:(BOOL) useNativeUI;
-(void) cancelPendingInviteToPlayerWithId:(NSString*) playerId;

-(void) cancelMatchSeartch;
-(void) finishMatchmaking;

-(void) queryActivity;
-(void) queryPlayerGroupActivity:(int) group;

-(void) startBrowsingForNearbyPlayers;
-(void) stopBrowsingForNearbyPlayers;

-(void) rematch;
-(void) disconnect;

-(void) sendData:(NSData *)data toPlayersWithIds:(NSArray *)toPlayersWithIds withDataMode:(GKMatchSendDataMode)withDataMode;
-(void) sendDataToAll:(NSData *)data withDataMode:(GKMatchSendDataMode)withDataMode;

@end
