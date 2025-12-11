# Changelog

## [1.5.0] - 2025-12-05
### Changed
- Update package CsvHelper to version 33.1.0.

## [1.4.0] - 2025-10-08
### Added
- Added option to ignore quotes

## [1.3.0] - 2024-08-20
### Added
- Updated NewtonSoft.Json to the latest version 13.0.3.

## [1.2.1] - 2023-12-20
### Fixed
- Empty and whitespace header values are now replaced if ReplaceHeaderWhitespaceWith is used.

## [1.2.0] - 2023-12-20
### Changed
- Type of Xml-property of Result-object changed from object to string.

### Fixed
- Task now produces valid XML in case no header row is provided in input CSV string.

## [1.1.2] - 2023-11-28
### Fixed
- Fixed documentational issues and changed Jtoken result object type to dynamic.

## [1.1.1] - 2023-11-23
### Fixed
- Fixed issue with inverted Trim option by inverting the if statement.

## [1.1.0] - 2023-07-31
### Added
- Implements option for treating missing fields as null when parsing the CSV content

## [1.0.0] - 2023-02-10
### Added
- Initial implementation
