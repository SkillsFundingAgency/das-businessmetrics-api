## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

# das-businessmetrics-api

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_apis/build/status/das-businessmetrics-api?branchName=main)](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_build/latest?definitionId=das-businessmetrics-api&branchName=main)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-businessmetrics-api&metric=alert_status)](https://sonarcloud.io/dashboard?id=SkillsFundingAgency_das-businessmetrics-api)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)

The business metrics api is a facade for retrieving data from log analytics that has been added by different applications

## How It Works

The business metrics api relies on data that has been added from other services to the log analytics workspace. This API is then able to query
that data over a specific time range. The available metric names are held in a static list in config as shown below. The current implementation
is only to get data needed for Find an apprenticeship for Recruit an apprentice to keep in its data store to show the performance of
vacancies published by providers or employers.

## 🚀 Installation

### Pre-Requisites

* A clone of this repository
* A code editor that supports Azure functions and .net 8
* An Azure Active Directory account with the appropriate roles as per the [config](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-businmetrics-api/SFA.DAS.BusinessMetrics.API.json)

### Config

This utility uses the standard Apprenticeship Service configuration. All configuration can be found in the [das-employer-config repository](https://github.com/SkillsFundingAgency/das-employer-config).

* A log analytics workspace identifier is required in the table storage configuration

AppSettings.json file
```json
{
    "Logging": {
      "LogLevel": {
        "Default": "Information",
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Information"
      }
    },
    "ConfigurationStorageConnectionString": "UseDevelopmentStorage=true;",
    "ConfigNames": "SFA.DAS.BusinessMetrics.Api",
    "EnvironmentName": "LOCAL",
    "Version": "1.0",
    "APPINSIGHTS_INSTRUMENTATIONKEY": ""
  }  
```

Azure Table Storage config

Row Key: SFA.DAS.BusinessMetrics.Api_1.0

Partition Key: LOCAL

Data:

```json
{
  "AzureAd": {
    "tenant": "{TENANT_NAME}.onmicrosoft.com",
    "identifier": "https://{TENANT_NAME}.onmicrosoft.com/{SERVICE_NAME}"
  },
  "LogAnalyticsWorkSpace": {
    "Identifier": "{ANALYTICS_WORK_SPACE}"
  },
  "MetricsConfiguration": {
    "CustomMetrics": [
      {
        "ServiceName": "VacanciesOuterApi",
        "CounterName": "Vacancies.vacancyReference"
      },
      {
        "ServiceName": "FindAnApprenticeshipOuterApi",
        "CounterName": "FindAnApprenticeship.vacancyReference"
      }
    ]
  }
}
```

## 🔗 External Dependencies

This API relies on access to a log analytics workspace.

## Technologies

* .net 8
* Azure Table Storage
* NUnit
* Moq
* FluentAssertions
* Log Analytics


## 🐛 Known Issues