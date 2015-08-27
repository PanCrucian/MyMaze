//
//  SocialGate.h
//  Unity-iPhone
//
//  Created by lacost on 2/15/14.
//
//

#import <Foundation/Foundation.h>
#import <Accounts/Accounts.h>
#import <Social/Social.h>
#import <MessageUI/MessageUI.h> 

#if UNITY_VERSION < 450
#include "iPhone_View.h"
#endif



#include "ISNDataConvertor.h"

@interface SocialGate : NSObject<MFMailComposeViewControllerDelegate>

+ (id) sharedInstance;

- (void) twitterPost:(NSString*)status url: (NSString*) url media: (NSString*) media;
- (void) fbPost:(NSString*)status  url: (NSString*) url media: (NSString*) media;



- (void) mediaShare:(NSString*)text media: (NSString*) media;
- (void) sendEmail:(NSString*)subject body: (NSString*) body recipients: (NSString*) recipients media: (NSString*) media;
@end
