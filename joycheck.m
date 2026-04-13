#import <Foundation/Foundation.h>
#import <GameController/GameController.h>

int main(int argc, const char * argv[]) {
    @autoreleasepool {
        // List controllers immediately — no app loop
        NSArray *controllers = [GCController controllers];
        if (controllers.count == 0) {
            printf("No controllers detected\n");
        } else {
            for (GCController *controller in controllers) {
                NSString *name = controller.vendorName ?: @"Unknown";
                printf("Controller: %s\n", [name UTF8String]);
            }
        }
    }
    return 0;
}
