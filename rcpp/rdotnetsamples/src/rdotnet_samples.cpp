#include <string>
#include <Rcpp.h>
#include <sstream>

#include "rdotnet_samples.h"

using namespace Rcpp;

// [[Rcpp::export]]
void broadcast_progress_update(CharacterVector message, NumericVector percentage) {
	std::string msg = as<std::string>(message);
	double p = percentage[0];
	if(progress_handler != nullptr)
		(*progress_handler)(msg.c_str(), p);
}

// [[Rcpp::export]]
void register_default_progress_handler()
{
	register_progress_handler((void*)&default_progress_handler);
}

namespace patch
{
	// work around a std::to_string mingw bug that seems 
	// present with the old GCC that RTools on windows still uses.
	// http://stackoverflow.com/questions/12975341/to-string-is-not-a-member-of-std-says-so-g
    template < typename T > std::string to_string( const T& n )
    {
        std::ostringstream stm ;
        stm << n ;
        return stm.str() ;
    }
}

void default_progress_handler(const char * message, double percentage)
{
	std::string msg(message);
	std::string p = patch::to_string<double>(percentage);
	msg = std::string("Default R progress msg: ") + msg;
	Rprintf( "%s : %s\n", p.c_str(), msg.c_str() );
}

void register_progress_handler(void* handler)
{
	progress_handler = (progress_callback)handler;
}

