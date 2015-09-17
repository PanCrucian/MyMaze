//=============================================================================
//  UnityADC.mm
//
//  iOS functionality for the Unity AdColony plug-in.
//
//  Copyright 2010 Jirbo, Inc.  All rights reserved.
//
//  ---------------------------------------------------------------------------
//
//  * Instructions *
//
//  Copy this file into your Unity project's Assets/Plugins/iOS folder.
//
//  Refer to the header comment in AdColony.cs for further instructions.
//
//=============================================================================

#import <AdColony/AdColony.h>

void UnityPause(bool pause);


@interface UnityADCIOSDelegate : NSObject<AdColonyDelegate,AdColonyAdDelegate>
{
}
// AdColonyDelegate
- (void) onAdColonyV4VCReward:(BOOL)success currencyName:(NSString *)currencyName currencyAmount:(int)amount inZone:(NSString *)zoneID;
- (void) onAdColonyAdAvailabilityChange:(BOOL)available inZone:(NSString *)zoneID;

// AdColonyAdDelegate
- (void) onAdColonyAdStartedInZone:(NSString *)zoneID;
- (void) onAdColonyAdFinishedWithInfo:( AdColonyAdInfo * )info;

@end

NSString*     adc_app_version = nil;
NSString*     adc_app_id = nil;
NSString*     adc_cur_zone = nil;
NSMutableArray* adc_zone_ids = nil;
UnityADCIOSDelegate* adc_ios_delegate = nil;
UIViewController* appViewController = nil;

NSString* set_adc_cur_zone( NSString* new_adc_cur_zone )
{
  if (adc_cur_zone) [adc_cur_zone release];
  adc_cur_zone = [new_adc_cur_zone retain];
  return adc_cur_zone;
}


@implementation UnityADCIOSDelegate
// AdColonyDelegate
- (void) onAdColonyV4VCReward:(BOOL)success currencyName:(NSString *)currencyName currencyAmount:(int)amount inZone:(NSString *)zoneID
{
  NSString* success_str = success ? @"true" : @"false";
  UnitySendMessage( "AdColony", "OnAdColonyV4VCResult",
      //test comment
      [[NSString stringWithFormat:@"%@|%d|%@", success_str, amount, currencyName] UTF8String] );
}

- (void) onAdColonyAdAvailabilityChange:(BOOL)available inZone:(NSString *)zoneID
{
  NSString* available_str = available ? @"true" : @"false";
  UnitySendMessage( "AdColony", "OnAdColonyAdAvailabilityChange",
      [[NSString stringWithFormat:@"%@|%@", available_str, zoneID] UTF8String] );
}

// AdColonyAdDelegate
- (void) onAdColonyAdStartedInZone:(NSString *)zoneID
{
  UnitySendMessage( "AdColony", "OnAdColonyVideoStarted", "" );
}

- (void) onAdColonyAdFinishedWithInfo:(AdColonyAdInfo *)info
{
    const char* message_info = [[NSString stringWithFormat:@"%@|%@|%d|%@", info.shown ? @"true" : @"false", info.iapEnabled ? @"true" : @"false", info.iapEngagementType, info.iapProductID ] UTF8String];
  NSLog(@"%s", message_info);
  UnitySendMessage( "AdColony", "OnAdColonyVideoFinished", message_info);
    [[UIApplication sharedApplication] keyWindow].rootViewController = appViewController;
}
@end

#include <iostream>
using namespace std;

extern "C" {
  void IOSSetCustomID( const char* custom_id ) {
    NSString* custom_id_nsstr = [NSString stringWithUTF8String:custom_id];
    [AdColony setCustomID:custom_id_nsstr];
  }

  char* IOSGetCustomID() {
    NSString* result_str = [AdColony getCustomID];
    if (result_str) {
      const char *c_str = [result_str UTF8String];
      size_t count = strlen( c_str );
      char* result = (char *)malloc(count + 1);
      strcpy( result, c_str );
      return result;
    } else {
      char* result = new char[10];
      strcpy( result, "undefined" );
      return result;
    }
  }

  void  IOSConfigure( const char* app_version, const char* app_id, int zone_id_count, const char* zone_ids[] ) {
    adc_app_version = [[NSString stringWithUTF8String:app_version] retain];
    adc_app_id = [[NSString stringWithUTF8String:app_id] retain];

    adc_zone_ids = [[[NSMutableArray alloc] initWithCapacity:zone_id_count] retain];
    for (int i=0; i < zone_id_count; ++i) {
      NSString* zone_id_str = [NSString stringWithUTF8String:zone_ids[i]];
      [adc_zone_ids addObject:zone_id_str];
      if (i == 0) {
        set_adc_cur_zone( zone_id_str );
      }
    }

    adc_ios_delegate = [[[UnityADCIOSDelegate alloc] init] retain];
    [AdColony configureWithAppID:adc_app_id zoneIDs:adc_zone_ids delegate:adc_ios_delegate logging:NO];
  }

  bool  IOSIsVideoAvailable( const char* zone_id ) {
    NSString* zid = adc_cur_zone;
    if (zone_id && zone_id[0] != 0) {
      zid = set_adc_cur_zone( [NSString stringWithUTF8String:zone_id] );
    }
    return [AdColony zoneStatusForZone:zid] == ADCOLONY_ZONE_STATUS_ACTIVE;
  }

  bool  IOSIsV4VCAvailable( const char* zone_id ) {
    NSString* zid = adc_cur_zone;
    if (zone_id && zone_id[0] != 0) {
      zid = set_adc_cur_zone( [NSString stringWithUTF8String:zone_id] );
    }
    if ( !IOSIsVideoAvailable(zone_id) ) {
      return false;
    }
    return [AdColony isVirtualCurrencyRewardAvailableForZone:zid];
  }

  char* IOSGetDeviceID()
  {
    NSString* result_str = [AdColony getUniqueDeviceID];
    if (result_str) {
      const char *c_str = [result_str UTF8String];
      size_t count = strlen( c_str );
      char* result = (char *)malloc(count + 1);
      strcpy( result, c_str );
      return result;
    } else {
      char* result = new char[10];
      strcpy( result, "undefined" );
      return result;
    }
  }

  char* IOSGetOpenUDID() {
    NSString* result_str = [AdColony getOpenUDID];
    if (result_str) {
      const char *c_str = [result_str UTF8String];
      size_t count = strlen( c_str );
      char* result = (char *)malloc(count + 1);
      strcpy( result, c_str );
      return result;
    } else {
      char* result = new char[10];
      strcpy( result, "undefined" );
      return result;
    }
  }

  char* IOSGetODIN1() {
    NSString* result_str = [AdColony getODIN1];
    if (result_str) {
      const char *c_str = [result_str UTF8String];
      size_t count = strlen( c_str );
      char* result = (char *)malloc(count + 1);
      strcpy( result, c_str );
      return result;
    } else {
      char* result = new char[10];
      strcpy( result, "undefined" );
      return result;
    }
  }

  int   IOSGetV4VCAmount( const char* zone_id ) {
    NSString* zid = adc_cur_zone;
    if (zone_id && zone_id[0] != 0) {
      zid = set_adc_cur_zone( [NSString stringWithUTF8String:zone_id] );
    }
    return [AdColony getVirtualCurrencyRewardAmountForZone:zid];
  }

  char* IOSGetV4VCName( const char* zone_id ) {
    NSString* zid = adc_cur_zone;
    if (zone_id && zone_id[0] != 0) {
      zid = set_adc_cur_zone( [NSString stringWithUTF8String:zone_id] );
    }
    NSString* result_str = [AdColony getVirtualCurrencyNameForZone:zid];
    if (result_str) {
      const char *c_str = [result_str UTF8String];
      size_t count = strlen( c_str );
      char* result = (char *)malloc(count + 1);
      strcpy( result, c_str );
      return result;
    } else {
      char* result = new char[10];
      strcpy( result, "undefined" );
      return result;
    }
  }

  char* IOSStatusForZone( const char* zone_id) {
    NSString* zid = adc_cur_zone;
    if (zone_id && zone_id[0] != 0) {
      zid = set_adc_cur_zone( [NSString stringWithUTF8String:zone_id] );
    }

    char* status_cstr = (char*)malloc(10);
    strcpy(status_cstr, "");
    // NSString* status_str;
    ADCOLONY_ZONE_STATUS status = [AdColony zoneStatusForZone:zid];
    switch (status) {
      case ADCOLONY_ZONE_STATUS_NO_ZONE:
        strcpy(status_cstr, "invalid");
        break;
      case ADCOLONY_ZONE_STATUS_OFF:
        strcpy(status_cstr, "off");
        break;
      case ADCOLONY_ZONE_STATUS_ACTIVE:
        strcpy(status_cstr, "active");
        break;
      case ADCOLONY_ZONE_STATUS_LOADING:
        strcpy(status_cstr, "loading");
        break;
      case ADCOLONY_ZONE_STATUS_UNKNOWN:
        strcpy(status_cstr, "unknown");
        break;
    }

    return status_cstr;
  }

  bool  IOSShowVideoAd( const char* zone_id ) {
    NSString* zid = adc_cur_zone;
    if (zone_id && zone_id[0] != 0) {
      zid = set_adc_cur_zone( [NSString stringWithUTF8String:zone_id] );
    }
    if ( !IOSIsVideoAvailable(zone_id) ) {
      return false;
    }
      UIWindow* window = [[UIApplication sharedApplication] keyWindow];
      UIViewController* viewController = [UIViewController new];
      [viewController.view setBackgroundColor:[UIColor whiteColor]];
      appViewController = window.rootViewController;
      window.rootViewController = viewController;

    [AdColony playVideoAdForZone:zid withDelegate:adc_ios_delegate];
    return true;
  }

  bool  IOSShowV4VC( bool popup_result, const char* zone_id ) {
    NSString* zid = adc_cur_zone;
    if (zone_id && zone_id[0] != 0) {
      zid = set_adc_cur_zone( [NSString stringWithUTF8String:zone_id] );
    }
    if ( !IOSIsV4VCAvailable(zone_id) ) {
      return false;
    }
      UIWindow* window = [[UIApplication sharedApplication] keyWindow];
      UIViewController* viewController = [UIViewController new];
      [viewController.view setBackgroundColor:[UIColor whiteColor]];
      appViewController = window.rootViewController;
      window.rootViewController = viewController;

    [AdColony playVideoAdForZone:zid withDelegate:adc_ios_delegate
                withV4VCPrePopup:NO andV4VCPostPopup:popup_result];
    return true;
  }

  void  IOSOfferV4VC( bool popup_result, const char* zone_id ) {
    NSString* zid = adc_cur_zone;
    if (zone_id && zone_id[0] != 0) {
      zid = set_adc_cur_zone( [NSString stringWithUTF8String:zone_id] );
    }
    if ( !IOSIsV4VCAvailable(zone_id) ) {
      return;
    }
      UIWindow* window = [[UIApplication sharedApplication] keyWindow];
      UIViewController* viewController = [UIViewController new];
      [viewController.view setBackgroundColor:[UIColor whiteColor]];
      appViewController = window.rootViewController;
      window.rootViewController = viewController;

    [AdColony playVideoAdForZone:zid withDelegate:adc_ios_delegate
                withV4VCPrePopup:YES andV4VCPostPopup:popup_result];
  }

  void IOSNotifyIAPComplete( const char* product_id, const char* trans_id, const char* currency_code, double price, int quantity) {
    NSString* ns_trans_id = [NSString stringWithUTF8String:trans_id];
    NSString* ns_product_id = [NSString stringWithUTF8String:product_id];
    NSString* ns_currency_code = @"";
    if (currency_code != NULL) {
      ns_currency_code = [NSString stringWithUTF8String:currency_code];
    }
    NSNumber* ns_price = [NSNumber numberWithDouble:price];
    [AdColony notifyIAPComplete:ns_trans_id productID:ns_product_id quantity:quantity price:ns_price currencyCode:ns_currency_code];
  }
}
