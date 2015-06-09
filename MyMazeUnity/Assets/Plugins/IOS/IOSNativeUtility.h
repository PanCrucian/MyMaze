//
//  IOSNativeUtility.h
//  Unity-iPhone
//
//  Created by Osipov Stanislav on 4/29/14.
//
//

#import <Foundation/Foundation.h>
#import "ISNDataConvertor.h"
#if UNITY_VERSION < 450
#include "iPhone_View.h"
#endif

@interface IOSNativeUtility : NSObject

@property (strong)  UIActivityIndicatorView *spinner;

 + (id) sharedInstance;

 - (void) redirectToRatingPage: (NSString *) appId;

 - (void) ShowSpinner;
 - (void) HideSpinner;
- (void) GetIFA;


@end