# Changelog for Relatiity Test Helpers

- This file is used to list changes made in the relativity-test-helpers repo.
- **Place Newer updates on the top**

-------------------------

## 2021-03-04
- REL-518429 - Removed RSAPI from User Helper classes and moved their main code into UserHelper.  Updated tests as well to account for this.
    - Converged all usage of ServiceFactory & IServicesMgr to IServicesMgr
    - Removed various usages of RSAPI across many projects

-------------------------

## 2021-03-03
- REL-518432 - Removed RSAPI address from the Configuration Helper. Switch current uses to use Instance Address instead.
- REL-530844 - Removed DbContext usage from methods in ArtifactHelpers folder.

-------------------------

## 2021-03-02
- REL-518430 - Removed RSAPI from Workspace classes and moved their main code into WorkspaceHelpers.  Updated tests as well to account for this.

-------------------------

## 2021-03-01
- REL-518428 - Removed RSAPI from Group class and renamed to GroupHelper class. Also updated integration tests.

-------------------------

## 2021-02-26
- REL-518423 - Removed RSAPI from Folder class and renamed to FoldersHelper class. Also updated integration tests and indirect RSAPI usage in WorkspaceHelper

-------------------------

## 2021-02-22
- REL-518419 - Removed RSAPI from Client class and renamed to ClientHelpers class. Also updated integration tests.

-------------------------

## 2021-02-15
- REL-518412 - Removed NUnit dependency and deleted a constructor that accepted TestContext (an NUnit object).

