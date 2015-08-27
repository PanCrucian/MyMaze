//
//  ISN_Media.h
//  Unity-iPhone
//
//  Created by lacost on 6/24/15.
//
//

#import <Foundation/Foundation.h>
#import <MediaPlayer/MediaPlayer.h>
#import "ISNDataConvertor.h"


@interface ISN_Media : NSObject<MPMediaPickerControllerDelegate> {

MPMusicPlayerController		*musicPlayer;

}


@property (nonatomic, retain)   UIViewController *vc;

+ (ISN_Media *)sharedInstance;

- (void) initialize;
- (void) setShuffleMode: (MPMusicShuffleMode) mode;
- (void) setRepeatMode: (MPMusicRepeatMode) mode;


- (void) play;
- (void) pause;
- (void) skipToNextItem;
- (void) skipToBeginning;
- (void) skipToPreviousItem;



- (void) showMediaPicker;
- (void) setCollection: (NSArray*) itemsIds;



@end