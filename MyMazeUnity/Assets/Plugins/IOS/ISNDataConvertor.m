//
//  Unity3d.m
//
//  Created by Osipov Stanislav on 1/11/13.
//
//

#import "ISNDataConvertor.h"

NSString * const UNITY_SPLITTER = @"|";
NSString * const UNITY_SPLITTER2 = @"|%|";
NSString * const UNITY_EOF = @"endofline";

@implementation ISNDataConvertor




+(NSString *) charToNSString:(char *)value {
    if (value != NULL) {
        return [NSString stringWithUTF8String: value];
    } else {
        return [NSString stringWithUTF8String: ""];
    }
}

+(const char *)NSIntToChar:(NSInteger)value {
    NSString *tmp = [NSString stringWithFormat:@"%d", value];
    return [tmp UTF8String];
}

+ (const char *) NSStringToChar:(NSString *)value {
    return [value UTF8String];
}

+ (NSArray *)charToNSArray:(char *)value {
    NSString* strValue = [ISNDataConvertor charToNSString:value];
    
    NSArray *array;
    if([strValue length] == 0) {
        array = [[NSArray alloc] init];
    } else {
        array = [strValue componentsSeparatedByString:@"|"];
    }
    
    return array;
}

+ (const char *) NSStringsArrayToChar:(NSArray *) array {
    return [ISNDataConvertor NSStringToChar:[ISNDataConvertor serializeNSStringsArray:array]];
}

+ (NSString *) serializeNSStringsArray:(NSArray *) array {
    
    NSMutableString * data = [[NSMutableString alloc] init];
    

    for(NSString* str in array) {
        [data appendString:str];
        [data appendString: UNITY_SPLITTER];
    }
    
    [data appendString: UNITY_EOF];
    
    NSString *str = [data copy];
    #if UNITY_VERSION < 500
        [str autorelease];
    #endif
    
    return str;
}


+ (NSString *)serializeErrorToNSString:(NSError *)error {
    NSString* description = @"";
    if(error.description != nil) {
        description = error.description;
    }
    return  [self serializeErrorWithDataToNSString:description code:error.code];
}

+ (NSString *)serializeErrorWithDataToNSString:(NSString *)description code:(int)code {
    NSMutableString * data = [[NSMutableString alloc] init];
    
    
    [data appendString: [NSString stringWithFormat:@"%d", code]];
    [data appendString: UNITY_SPLITTER];
    [data appendString: description];
    
    
    NSString *str = [data copy];
#if UNITY_VERSION < 500
    [str autorelease];
#endif

    return  str;
}


+ (const char *) serializeErrorWithData:(NSString *)description code: (int) code {
    NSString *str = [ISNDataConvertor serializeErrorWithDataToNSString:description code:code];
    return [ISNDataConvertor NSStringToChar:str];
}

+ (const char *) serializeError:(NSError *)error  {
    NSString *str = [ISNDataConvertor serializeErrorToNSString:error];
    return [ISNDataConvertor NSStringToChar:str];
}



@end


