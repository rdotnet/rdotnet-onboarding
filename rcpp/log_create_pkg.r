# To build the package you must have a working installation of RTools, and the Rcpp package installed.
library(Rcpp)
# Package skeleton was created with:
pkgDir <- "C:/src/github_jm/rdotnet-onboarding/rcpp"
Rcpp.package.skeleton(name = "rdotnetsamples", list = character(), 
	environment = .GlobalEnv, path = pkgDir, force = FALSE, 
	code_files = character(), cpp_files = character(),
	example_code = TRUE, attributes = TRUE, module = FALSE, 
	author = "Jean-Michel Perraud", 
	email = "your@email.com", 
	license = "MIT"
)
pkgPath <- file.path(pkgDir, 'rdotnetsamples')  
compileAttributes(pkgPath)

# then e.g. in a windows command prompt:  
### cd c:\path\to\rdotnet-onboarding\rcpp
### "C:\Program Files\R\R-3.2.2\bin\x64\R.exe" --no-save --no-restore-data CMD build rdotnetsamples
### "C:\Program Files\R\R-3.2.2\bin\x64\R.exe" --no-save --no-restore-data CMD INSTALL rdotnetsamples_1.0.tar.gz


# sample usage test in R, before we use that from R.NET
library(rdotnetsamples)
rdotnetsamples::register_default_progress_handler()
rdotnetsamples::broadcast_progress_update("Yo!", 5)
getLoadedDLLs()$rdotnetsamples[['path']]

