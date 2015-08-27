
#import <Foundation/Foundation.h>
#import <GameKit/GameKit.h>
#import "ISN_GameCenterTBM.h"


@interface ISN_GameCenterListner : NSObject <GKLocalPlayerListener> {
    BOOL isInited;
    
}

+ (id) sharedInstance;

-(void) subscribe;



@end
