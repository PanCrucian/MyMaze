#import <Foundation/Foundation.h>
#import <GameKit/GameKit.h>

#import "ISN_GameCenterRTM.h"
#import "ISN_GameCenterTBM.h"
#import "ISN_GameCenterListner.h"
#if UNITY_VERSION < 450
#include "iPhone_View.h"
#endif


@protocol GameCenterManagerDelegate <NSObject>
@optional
- (void) processGameCenterAuth: (NSError*) error;
- (void) scoreReported: (NSError*) error;
- (void) reloadScoresComplete: (GKLeaderboard*) leaderboard error: (NSError*) error;
- (void) achievementSubmitted: (GKAchievement*) ach error:(NSError*) error;
- (void) achievementResetResult: (NSError*) error;
- (void) mappedPlayerIDToPlayer: (GKPlayer*) player error: (NSError*) error;
@end

@interface ISN_GameCenterManager : NSObject <GKGameCenterControllerDelegate, GKAchievementViewControllerDelegate> {
    BOOL isAchievementsWasLoaded;
    NSMutableDictionary* earnedAchievementCache;
    NSString* requestedLeaderboardId;
    int lbscope;
    
#if UNITY_VERSION < 500
    id <GameCenterManagerDelegate, NSObject> delegate;
#else
    id <GameCenterManagerDelegate, NSObject> __unsafe_unretained delegate;
#endif
}

//This property must be attomic to ensure that the cache is always in a viable state...
@property (retain) NSMutableDictionary* earnedAchievementCache;
@property (nonatomic, assign)  id <GameCenterManagerDelegate> delegate;


+ (ISN_GameCenterManager *) sharedInstance;

- (void) getSignature;

- (void) reportScore: (long long) score forCategory: (NSString*) category;

- (void) authenticateLocalPlayer;
- (void) showLeaderboard: (NSString*)leaderboardId scope: (int) scope;
- (void) retrieveScoreForLocalPlayerWithCategory:(NSString*)category scope: (int) scope collection: (int) collection;
- (void) retriveScores:(NSString*)category scope: (int) scope collection: (int) collection from: (int) from to: (int) to;



- (void) savePlayerInfo:(GKPlayer*) player;
- (void) loadPlayerInfoForPlayerWithId:(NSString *)playerId;
- (void) loadImageForPlayerWithPlayerId:(NSString *)playerId size:(GKPhotoSize) size;
-(GKPlayer*) getPlayerWithId:(NSString*) playerId;


- (NSString*) saveInvite:(GKInvite*) invite;
- (NSString*) serialiseInvite:(GKInvite*) invite;
- (GKInvite*) getInviteWithId:(NSString*)inviteId;


- (void) sendLeaderboardChallenge:(NSString*) leaderboardId message:(NSString*) message playerIds: (NSArray*) playerIds;
- (void) sendLeaderboardChallengeWithFriendsPicker:(NSString *)leaderboardId message:(NSString *)message;
    
- (void) sendAchievementChallenge:(NSString*) achievementId  message:(NSString*) message playerIds: (NSArray*) playerIds;
- (void) sendAchievementChallengeWithFriendsPicker:(NSString *)achievementId message:(NSString *)message;


- (void) showAchievements;
- (void) resetAchievements; 
- (void) submitAchievement: (double) percentComplete identifier: (NSString*) identifier notifyComplete: (BOOL) notifyComplete;

- (void) retrieveFriends;
- (void) loadLeaderboardSetInfo;
- (void) loadLeaderboardsForSet:(NSString *)uid;

- (BOOL) isGameCenterAvailable;





@end