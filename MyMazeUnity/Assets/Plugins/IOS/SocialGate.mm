//
//  SocialGate.m
//  Unity-iPhone
//
//  Created by lacost on 2/15/14.
//
//

#import "SocialGate.h"


@implementation SocialGate

static SocialGate *_sharedInstance;


+ (id)sharedInstance {
    
    if (_sharedInstance == nil)  {
        _sharedInstance = [[self alloc] init];
    }
    
    return _sharedInstance;
}

-(void) mediaShare:(NSString *)text  media:(NSString *)media {
    NSLog(@"ISN: mediaShare");
    UIActivityViewController *controller;
                                            
                                            
    if(media.length != 0) {
        NSData *imageData = [[NSData alloc] initWithBase64Encoding:media];
        UIImage *image = [[UIImage alloc] initWithData:imageData];
        
        //[UIPopoverPresentationController alloc] ini
        
         NSLog(@"ISN: image added");
        if(text.length != 0) {
              NSLog(@"ISN: text added");
            controller = [[UIActivityViewController alloc] initWithActivityItems:@[text, image] applicationActivities:nil];
        } else {
             NSLog(@"ISN: no text");
            controller = [[UIActivityViewController alloc] initWithActivityItems:@[image] applicationActivities:nil];
        }
        
    } else {
        NSLog(@"ISN: no media");
        controller = [[UIActivityViewController alloc] initWithActivityItems:@[text] applicationActivities:nil];
    }
    
    
   
    
    UIViewController *vc =  UnityGetGLViewController();
    
    
    NSArray *vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
    if ([[vComp objectAtIndex:0] intValue] >= 8) {
          NSLog(@"ISN: iOS8 detected");
        UIPopoverPresentationController *presentationController = [controller popoverPresentationController];
        presentationController.sourceView = vc.view;
    }
    
    [vc presentViewController:controller animated:YES completion:nil];
    
}

-(void) twitterPost:(NSString *)status url:(NSString *)url media:(NSString *)media {
    NSLog(@"ISN: twitterPost");
    
   
    
    

    [SLComposeServiceViewController attemptRotationToDeviceOrientation];
    SLComposeViewController *tweetSheet = [SLComposeViewController composeViewControllerForServiceType:SLServiceTypeTwitter];
    
    if(tweetSheet == NULL) {
        NSLog(@"ISN: SLServiceTypeTwitter not avaliable ");
        UnitySendMessage("IOSSocialManager", "OnTwitterPostFailed", [ISNDataConvertor NSStringToChar:@""]);
        return;
    }

    
    if(status.length > 0) {
        [tweetSheet setInitialText:status];
    }
    
    if(media.length > 0) {
        NSData *imageData = [[NSData alloc] initWithBase64Encoding:media];
        UIImage *image = [[UIImage alloc] initWithData:imageData];
        [tweetSheet addImage:image];
    }
   
    if(url.length > 0) {
        NSURL *urlObject = [NSURL URLWithString:url];
        [tweetSheet addURL:urlObject];
    }
    
    UIViewController *vc =  UnityGetGLViewController();
    [vc presentViewController:tweetSheet animated:YES completion:nil];
    
    tweetSheet.completionHandler = ^(SLComposeViewControllerResult result) {
        NSArray *vComp;
        switch(result) {
            //  This means the user cancelled without sending the Tweet
            case SLComposeViewControllerResultCancelled:
                vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
                if ([[vComp objectAtIndex:0] intValue] < 7) {
                    [tweetSheet dismissViewControllerAnimated:YES completion:nil];
                }
                
                
                NSLog(@"ISN: Tweet message was cancelled");
                UnitySendMessage("IOSSocialManager", "OnTwitterPostFailed", [ISNDataConvertor NSStringToChar:@""]);
                break;
                //  This means the user hit 'Send'
            case SLComposeViewControllerResultDone:
                NSLog(@"ISN: Done pressed successfully");
                
                vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
                if ([[vComp objectAtIndex:0] intValue] < 7) {
                    [tweetSheet dismissViewControllerAnimated:YES completion:nil];
                }
                
                UnitySendMessage("IOSSocialManager", "OnTwitterPostSuccess", [ISNDataConvertor NSStringToChar:@""]);
                break;
        }
    };

    
    
}



- (void) fbPost:(NSString *)status url:(NSString *)url media:(NSString *)media {
    NSLog(@"ISN: fbPostWithMedia");
    


    [SLComposeServiceViewController attemptRotationToDeviceOrientation];
    SLComposeViewController *fbSheet = [SLComposeViewController composeViewControllerForServiceType:SLServiceTypeFacebook];
    if(fbSheet == NULL) {
        NSLog(@"ISN: SLServiceTypeFacebook not avaliable ");
        UnitySendMessage("IOSSocialManager", "OnFacebookPostFailed", [ISNDataConvertor NSStringToChar:@""]);
        return;
    }
    
    
    if(status.length > 0) {
        [fbSheet setInitialText:status];
    }
    
    if(media.length > 0) {
        NSData *imageData = [[NSData alloc] initWithBase64Encoding:media];
        UIImage *image = [[UIImage alloc] initWithData:imageData];
        [fbSheet addImage:image];
    }
    
    if(url.length > 0) {
        NSURL *urlObject = [NSURL URLWithString:url];
        [fbSheet addURL:urlObject];
    }
    
    
    UIViewController *vc =  UnityGetGLViewController();
    
    [vc presentViewController:fbSheet animated:YES completion:nil];
    
    fbSheet.completionHandler = ^(SLComposeViewControllerResult result) {
        NSArray *vComp;
        switch(result) {
                
            case SLComposeViewControllerResultCancelled:
                
                vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
                if ([[vComp objectAtIndex:0] intValue] < 7) {
                    [fbSheet dismissViewControllerAnimated:YES completion:nil];
                }
                
                
                NSLog(@"ISN: Tweet message was cancelled");
                UnitySendMessage("IOSSocialManager", "OnFacebookPostFailed", [ISNDataConvertor NSStringToChar:@""]);
                break;
                //  This means the user hit 'Send'
            case SLComposeViewControllerResultDone:
                
                vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
                if ([[vComp objectAtIndex:0] intValue] < 7) {
                    [fbSheet dismissViewControllerAnimated:YES completion:nil];
                }
                
                
                NSLog(@"ISN: Done pressed successfully");
                UnitySendMessage("IOSSocialManager", "OnFacebookPostSuccess", [ISNDataConvertor NSStringToChar:@""]);
                break;
        }
        
    };

}



- (void) sendEmail:(NSString *)subject body:(NSString *)body recipients: (NSString*) recipients media:(NSString *)media {
    
    NSLog(@"ISN: sendEmail");

   
    //Create a string with HTML formatting for the email body
    NSMutableString *emailBody = [[NSMutableString alloc] initWithString:@"<html><body>"] ;
#if UNITY_VERSION < 500
    [emailBody retain];
#endif
    
    
    //Add some text to it however you want
    [emailBody appendString:@"<p>"];
    [emailBody appendString:body];
    [emailBody appendString:@"</p>"];
    
    
    /*
    if(media.length > 0) {
       // NSLog(@"media: %@",media);
      
        
        
        [emailBody appendString:[NSString stringWithFormat:@"<p><b><img src='data:image/png;base64,%@'></b></p>",media]];
    }
     */
   
    
    //close the HTML formatting
    [emailBody appendString:@"</body></html>"];
   // NSLog(@"emailBody: %@",emailBody);
    
    
    
    //Create the mail composer window
    MFMailComposeViewController *emailDialog = [[MFMailComposeViewController alloc] init];
    
    if(emailDialog == nil) {
        UnitySendMessage("IOSSocialManager", "OnMailFailed", [ISNDataConvertor NSStringToChar:@""]);
        return;
    }
    
    emailDialog.mailComposeDelegate = self;
    [emailDialog setSubject:subject];
    [emailDialog setMessageBody:emailBody isHTML:YES];
    
    if(media.length > 0) {
        NSData *imageData = [[NSData alloc] initWithBase64Encoding:media];
        [emailDialog addAttachmentData:imageData mimeType:@"image/png" fileName:@"Attachment"];
    }
    
    
    NSArray *emails = [recipients componentsSeparatedByString:@","];

    [emailDialog setToRecipients:emails];
    
    
    UIViewController *vc =  UnityGetGLViewController();
    
    [vc presentViewController:emailDialog animated:YES completion:nil];
#if UNITY_VERSION < 500
    [emailDialog release];
    [emailBody release];
#endif
    
    
}


#pragma private

- (NSString*) photoFilePath {
    return [NSString stringWithFormat:@"%@/%@",[NSHomeDirectory() stringByAppendingPathComponent:@"Documents"],@"tempinstgramphoto.igo"];
}


- (void) mailComposeController:(MFMailComposeViewController *)controller didFinishWithResult:(MFMailComposeResult)result error:(NSError *)error {
    switch (result)
    {
        case MFMailComposeResultCancelled:
            UnitySendMessage("IOSSocialManager", "OnMailFailed", [ISNDataConvertor NSStringToChar:@""]);
            NSLog(@"ISN: Mail cancelled");
            break;
        case MFMailComposeResultSaved:
             UnitySendMessage("IOSSocialManager", "OnMailFailed", [ISNDataConvertor NSStringToChar:@""]);
            NSLog(@"ISN: Mail saved");
            break;
        case MFMailComposeResultSent:
            UnitySendMessage("IOSSocialManager", "OnMailSuccess", [ISNDataConvertor NSStringToChar:@""]);
            NSLog(@"ISN: Mail sent");
            break;
        case MFMailComposeResultFailed:
            UnitySendMessage("IOSSocialManager", "OnMailFailed", [ISNDataConvertor NSStringToChar:@""]);
            NSLog(@"ISN: Mail sent failure: %@", [error localizedDescription]);
            break;
        default:
            UnitySendMessage("IOSSocialManager", "OnMailFailed", [ISNDataConvertor NSStringToChar:@""]);
            break;
    }
    
    UIViewController *vc =  UnityGetGLViewController();
    [vc dismissViewControllerAnimated:YES completion:NULL];
}


extern "C" {
    
    
    //--------------------------------------
	//  IOS Plugin Section
	//--------------------------------------
    
    
    void _ISN_TwPost(char* text, char* url, char* encodedMedia) {
        [[SocialGate sharedInstance] twitterPost:[ISNDataConvertor charToNSString:text] url:[ISNDataConvertor charToNSString:url] media:[ISNDataConvertor charToNSString:encodedMedia]];
    }
    
    
    void _ISN_FbPost(char* text, char* url, char* encodedMedia) {
        [[SocialGate sharedInstance] fbPost:[ISNDataConvertor charToNSString:text] url:[ISNDataConvertor charToNSString:url] media:[ISNDataConvertor charToNSString:encodedMedia]];
    }
    
    
    
    void _ISN_MediaShare(char* text, char* encodedMedia) {
        NSString *status = [ISNDataConvertor charToNSString:text];
        NSString *media = [ISNDataConvertor charToNSString:encodedMedia];
        
        [[SocialGate sharedInstance] mediaShare:status media:media];
        
    }
    
    
    void _ISN_SendMail(char* subject, char* body,  char* recipients, char* encodedMedia) {
        NSString *mailSubject       = [ISNDataConvertor charToNSString:subject];
        NSString *mailBody          = [ISNDataConvertor charToNSString:body];
        NSString *mailRecipients    = [ISNDataConvertor charToNSString:recipients];
        NSString *media             = [ISNDataConvertor charToNSString:encodedMedia];
        
        
        [[SocialGate sharedInstance] sendEmail:mailSubject body:mailBody recipients:mailRecipients media:media];
    }
    
    
    //--------------------------------------
    //  Mobile Social Plugin Section
    //--------------------------------------
    
    void _MSP_TwPost(char* text, char* url, char* encodedMedia) {
        _ISN_TwPost(text, url, encodedMedia);
    }
    
    
    void _MSP_FbPost(char* text, char* url, char* encodedMedia) {
        _ISN_FbPost(text, url, encodedMedia);
    }
    
    
    
    void _MSP_MediaShare(char* text, char* encodedMedia) {
        _ISN_MediaShare(text, encodedMedia);
    }
    
    
    void _MSP_SendMail(char* subject, char* body,  char* recipients, char* encodedMedia) {
        _ISN_SendMail(subject, body,  recipients, encodedMedia);
    }
    
}



@end
