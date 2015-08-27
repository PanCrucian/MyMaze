//
//  IOSNativePopUpsManager.m
//  Unity-iPhone
//
//  Created by Osipov Stanislav on 5/31/14.
//
//

#import "ISN_NativePopUpsManager.h"
#import "ISN_NativeUtility.h"

@implementation ISN_NativePopUpsManager



static UIAlertController* _currentAlert =  nil;


static ISN_NativePopUpsManager *_sharedInstance;

+ (id)sharedInstance {
    if (_sharedInstance == nil)  {
        _sharedInstance = [[self alloc] init];
    }
    
    return _sharedInstance;
}



+(void) dismissCurrentAlert {
    if(_currentAlert != nil) {
        [_currentAlert dismissViewControllerAnimated:true completion:^{
              UnitySendMessage("IOSPopUp", "onPopUpCallBack", [ISNDataConvertor NSStringToChar:@"0"]);
              UnitySendMessage("IOSRateUsPopUp", "onPopUpCallBack", [ISNDataConvertor NSStringToChar:@"0"]);
        }];
        
        
#if UNITY_VERSION < 500
        [_currentAlert release];
#endif
        _currentAlert = nil;
    }
}

+(void) showRateUsPopUp: (NSString *) title message: (NSString*) msg b1: (NSString*) b1 b2: (NSString*) b2 b3: (NSString*) b3 {
    
    UIAlertController* alert = [UIAlertController alertControllerWithTitle:title  message:msg  preferredStyle:UIAlertControllerStyleAlert];
    
    
    
    UIAlertAction* rateAction = [UIAlertAction actionWithTitle:b1 style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
        UnitySendMessage("IOSRateUsPopUp", "onPopUpCallBack", [ISNDataConvertor NSStringToChar:@"0"]);
        _currentAlert = nil;
    }];
    
    
    UIAlertAction* laterAction = [UIAlertAction actionWithTitle:b2 style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
        UnitySendMessage("IOSRateUsPopUp", "onPopUpCallBack", [ISNDataConvertor NSStringToChar:@"1"]);
        _currentAlert = nil;
    }];
    
    
    UIAlertAction* declineAction = [UIAlertAction actionWithTitle:b3 style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
        UnitySendMessage("IOSRateUsPopUp", "onPopUpCallBack", [ISNDataConvertor NSStringToChar:@"2"]);
        _currentAlert = nil;
    }];

    
    [alert addAction:rateAction];
    [alert addAction:laterAction];
    [alert addAction:declineAction];
    
    _currentAlert = alert;
    
    
    UIViewController *vc =  UnityGetGLViewController();
    [vc presentViewController:alert animated:YES completion:nil];
    
   
    
}




+ (void) showDialog: (NSString *) title message: (NSString*) msg yesTitle:(NSString*) b1 noTitle: (NSString*) b2{
    
    UIAlertController* alert = [UIAlertController alertControllerWithTitle:title  message:msg  preferredStyle:UIAlertControllerStyleAlert];
    
    UIAlertAction* okAction = [UIAlertAction actionWithTitle:b1 style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
        UnitySendMessage("IOSPopUp", "onPopUpCallBack", [ISNDataConvertor NSStringToChar:@"0"]);
        _currentAlert = nil;
    }];
    
    
    UIAlertAction* yesAction = [UIAlertAction actionWithTitle:b2 style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
        UnitySendMessage("IOSPopUp", "onPopUpCallBack", [ISNDataConvertor NSStringToChar:@"1"]);
        _currentAlert = nil;
    }];
    
    [alert addAction:yesAction];
    [alert addAction:okAction];
    
    _currentAlert = alert;
   
    
    UIViewController *vc =  UnityGetGLViewController();
    [vc presentViewController:alert animated:YES completion:nil];

}


+(void) showMessage: (NSString *) title message: (NSString*) msg okTitle:(NSString*) b1 {
    
    
    UIAlertController* alert = [UIAlertController alertControllerWithTitle:title  message:msg  preferredStyle:UIAlertControllerStyleAlert];
    
    UIAlertAction* defaultAction = [UIAlertAction actionWithTitle:b1 style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
        UnitySendMessage("IOSPopUp", "onPopUpCallBack", [ISNDataConvertor NSStringToChar:@"0"]);
        _currentAlert = nil;
    }];
    
    
    [alert addAction:defaultAction];
    _currentAlert = alert;
    
    
    UIViewController *vc =  UnityGetGLViewController();
    [vc presentViewController:alert animated:YES completion:nil];
    
}

//--------------------------------------
//  IOS 6,7 implementation
//--------------------------------------

static UIAlertView* _currentAllert =  nil;

+ (void) unregisterAllertView_old {
    if(_currentAllert != nil) {
#if UNITY_VERSION < 500
        [_currentAlert release];
#endif
        _currentAllert = nil;
    }
}

+(void) dismissCurrentAlert_old {
    if(_currentAllert != nil) {
        [_currentAllert dismissWithClickedButtonIndex:0 animated:YES];
#if UNITY_VERSION < 500
        [_currentAlert release];
#endif
        _currentAllert = nil;
    }
}

+(void) showRateUsPopUp_old: (NSString *) title message: (NSString*) msg b1: (NSString*) b1 b2: (NSString*) b2 b3: (NSString*) b3 {
    
    UIAlertView *alert = [[UIAlertView alloc] init];
    [alert setTitle:title];
    [alert setMessage:msg];
    [alert setDelegate: [ISN_NativePopUpsManager sharedInstance]];
    
    [alert addButtonWithTitle:b1];
    [alert addButtonWithTitle:b2];
    [alert addButtonWithTitle:b3];
    
    [alert show];
    
    _currentAllert = alert;
    
}




+ (void) showDialog_old: (NSString *) title message: (NSString*) msg yesTitle:(NSString*) b1 noTitle: (NSString*) b2{
    
    UIAlertView *alert = [[UIAlertView alloc] init];
    [alert setTitle:title];
    [alert setMessage:msg];
    [alert setDelegate: [ISN_NativePopUpsManager sharedInstance]];
    [alert addButtonWithTitle:b1];
    [alert addButtonWithTitle:b2];
    [alert show];
    
    _currentAllert = alert;
    
}


+(void) showMessage_old: (NSString *) title message: (NSString*) msg okTitle:(NSString*) b1 {
    
    UIAlertView *alert = [[UIAlertView alloc] init];
    [alert setTitle:title];
    [alert setMessage:msg];
    [alert setDelegate: [ISN_NativePopUpsManager sharedInstance]];
    [alert addButtonWithTitle:b1];
    [alert show];
    
    _currentAllert = alert;
}





- (void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex {
    [ISN_NativePopUpsManager unregisterAllertView_old];
    UnitySendMessage("IOSPopUp", "onPopUpCallBack",  [ISNDataConvertor NSIntToChar:buttonIndex]);
    UnitySendMessage("IOSRateUsPopUp", "onPopUpCallBack",  [ISNDataConvertor NSIntToChar:buttonIndex]);
}


extern "C" {

    
    //--------------------------------------
    //  IOS Native Plugin Section
    //--------------------------------------
    
    void _ISN_ShowRateUsPopUp(char* title, char* message, char* b1, char* b2, char* b3) {
        
        if([ISN_NativeUtility majorIOSVersion] >= 8) {
             [ISN_NativePopUpsManager showRateUsPopUp:[ISNDataConvertor charToNSString:title] message:[ISNDataConvertor charToNSString:message] b1:[ISNDataConvertor charToNSString:b1] b2:[ISNDataConvertor charToNSString:b2] b3:[ISNDataConvertor charToNSString:b3]];
        } else {
             [ISN_NativePopUpsManager showRateUsPopUp_old:[ISNDataConvertor charToNSString:title] message:[ISNDataConvertor charToNSString:message] b1:[ISNDataConvertor charToNSString:b1] b2:[ISNDataConvertor charToNSString:b2] b3:[ISNDataConvertor charToNSString:b3]];
        }
        
        
        
       
    }
    
    
    void _ISN_ShowDialog(char* title, char* message, char* yes, char* no) {
        if([ISN_NativeUtility majorIOSVersion] >= 8) {
         [ISN_NativePopUpsManager showDialog:[ISNDataConvertor charToNSString:title] message:[ISNDataConvertor charToNSString:message] yesTitle:[ISNDataConvertor charToNSString:yes] noTitle:[ISNDataConvertor charToNSString:no]];
        } else {
            [ISN_NativePopUpsManager showDialog_old:[ISNDataConvertor charToNSString:title] message:[ISNDataConvertor charToNSString:message] yesTitle:[ISNDataConvertor charToNSString:yes] noTitle:[ISNDataConvertor charToNSString:no]];

        }
        
    }
    
    void _ISN_ShowMessage(char* title, char* message, char* ok) {
         if([ISN_NativeUtility majorIOSVersion] >= 8) {
             [ISN_NativePopUpsManager showMessage:[ISNDataConvertor charToNSString:title] message:[ISNDataConvertor charToNSString:message] okTitle:[ISNDataConvertor charToNSString:ok]];
         } else {
             [ISN_NativePopUpsManager showMessage_old:[ISNDataConvertor charToNSString:title] message:[ISNDataConvertor charToNSString:message] okTitle:[ISNDataConvertor charToNSString:ok]];
         }
        
        
    }
    
    void _ISN_DismissCurrentAlert() {
        if([ISN_NativeUtility majorIOSVersion] >= 8) {
            [ISN_NativePopUpsManager dismissCurrentAlert];
        } else {
             [ISN_NativePopUpsManager dismissCurrentAlert_old];
        }
        
    }
    
    
    //--------------------------------------
    //  Native PopUps Plugin Section
    //--------------------------------------
    
    void _MNP_ShowRateUsPopUp(char* title, char* message, char* b1, char* b2, char* b3) {
        _ISN_ShowRateUsPopUp(title, message, b1, b2, b3);
    }
    
    
    void _MNP_ShowDialog(char* title, char* message, char* yes, char* no) {
        _ISN_ShowDialog(title, message, yes, no);
    }
    
    void _MNP_ShowMessage(char* title, char* message, char* ok) {
        _ISN_ShowMessage(title, message, ok);
    }
    
    void _MNP_DismissCurrentAlert() {
        _ISN_DismissCurrentAlert();
    }

    
    
}




@end
