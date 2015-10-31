
#define RDN_SAMPLE_EXPORT extern "C" __declspec(dllexport) 

typedef void(*progress_callback)(const char * message, double percentage);
progress_callback progress_handler = nullptr;

RDN_SAMPLE_EXPORT void default_progress_handler(const char * message, double percentage);
// The argument of register_progress_handler is a void* to make it an exportable function.
RDN_SAMPLE_EXPORT void register_progress_handler(void* handler);