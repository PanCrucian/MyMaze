#import <GameKit/GameKit.h>
#import <Foundation/Foundation.h>
#import "ISNDataConvertor.h"
#import "ISN_GameCenterManager.h"


@interface ISN_GameCenterTBM : NSObject<GKTurnBasedMatchmakerViewControllerDelegate, GKTurnBasedEventListener>

@property (nonatomic, retain) UIViewController *vc;

+ (id) sharedInstance;


-(void) loadMatches;
-(void) loadMatch:(NSString*) matchId;

-(void) findMatch:(int)minPlayers maxPlayers:(int) maxPlayers inviteMessage:(NSString*) inviteMessage invitationsList:(NSArray*) invitationsList;
-(void) findMatchWithNativeUI:(int)minPlayers maxPlayers:(int) maxPlayers inviteMessage:(NSString*) inviteMessage invitationsList:(NSArray*) invitationsList;

-(void) saveCurrentTurn:(NSString*) matchId updatedMatchData: (NSData *) updatedMatchData;


-(void) endTurn:(NSString*) matchId updatedMatchData: (NSData *) updatedMatchData nextPlayerId:(NSString*) nextPlayerId;


-(void) quitInTurn: (NSString*) matchId outcome:(int) outcome  nextPlayerId:(NSString*)nextPlayerId matchData: (NSData *) matchData;
-(void) quitOutOfTurn: (NSString*) matchId outcome:(int) outcome;

-(void) updatePlayerOutcome: (NSString*) matchId Outcome:(int) Outcome playerId:(NSString*) playerId;
-(void) endMatch:(NSString*) matchId matchData: (NSData *) matchData;
-(void) rematch:(NSString*) matchId;
-(void) removeMatch:(NSString*) matchId;



-(NSString*) serializeMathcData:(GKTurnBasedMatch *)match;
-(void) updateMatchInfo:(GKTurnBasedMatch *)match;

@end