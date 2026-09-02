# Changelog

## [1.4.0] - 2026-08-20
### Changed
- Upgraded target framework from .NET 6 to .NET 8.
- Added error handling support with ThrowErrorOnFailure and ErrorMessageOnFailure options.
- Added Error property to the Result object for accessing error details when ThrowErrorOnFailure is false.

## [1.3.0] - 2026-05-22
### Changed
- Update package Newtonsoft.Json to version 13.0.4.
- Improved memory usage and performance when converting large CSV files by implementing streaming JSON output. 

## [1.2.0] - 2025-05-12
### Changed
- Update package CsvHelper to version 33.1.0.

## [1.1.0] - 2025-11-18
### Added
- Added IgnoreQuotes parameter to Options class. When set to true, quotes are treated as regular characters, allowing proper handling of CSV data with special characters in property names.

## [1.0.0] - 2023-08-25
### Changed
- Initial implementation
