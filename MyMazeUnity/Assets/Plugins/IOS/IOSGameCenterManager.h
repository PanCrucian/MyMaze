#import <Foundation/Foundation.h>
#import <GameKit/GameKit.h>
#import "GameCenterManager.h"
#import "GCHelper.h"
#if UNITY_VERSION < 450
#include "iPhone_View.h"
#endif



@interface IOSGameCenterManager : NSObject <GKGameCenterControllerDelegate, GKAchievementViewControllerDelegate> {
    BOOL isAchievementsWasLoaded;
    NSString* requestedLeaderboardId;
    int lbscope;
    GameCenterManager *gameCenterManager;
    
    
}

- (void) getSignature;

- (void) reportScore: (long long) score forCategory: (NSString*) category;

- (void) authenticateLocalPlayer;
- (void) showLeaderboard: (NSString*)leaderboardId scope: (int) scope;
- (void) retrieveScoreForLocalPlayerWithCategory:(NSString*)category scope: (int) scope collection: (int) collection;
- (void) retriveScores:(NSString*)category scope: (int) scope collection: (int) collection from: (int) from to: (int) to;
- (void) loadUserData:(NSString *)uid;


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

- (BOOL)isGameCenterAvailable;





@end