//
//  Unity3d.h
//
//  Created by Osipov Stanislav on 1/11/13.
//
//

#import <Foundation/Foundation.h>


@interface ISNDataConvertor : NSObject

+ (NSString*) charToNSString: (char*)value;
+ (const char *) NSIntToChar: (NSInteger) value;
+ (const char *) NSStringToChar: (NSString *) value;
+ (NSArray*) charToNSArray: (char*)value;

+ (const char *) serializeErrorWithData:(NSString *)description code: (int) code;
+ (const char *) serializeError:(NSError *)error;

+ (NSString *) serializeErrorWithDataToNSString:(NSString *)description code: (int) code;
+ (NSString *) serializeErrorToNSString:(NSError *)error;


+ (const char *) NSStringsArrayToChar:(NSArray *) array;
+ (NSString *) serializeNSStringsArray:(NSArray *) array;

@end

