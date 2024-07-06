
var base_url = document.location.origin + '';
var CRMApp = angular.module('CRMApp', ['ui.bootstrap', 'ngMaterial', 'ngMessages', 'ui.router', 'ngCookies', 'popup.services', 'ngAnimate', 'ngLodash', 'ui.slimscroll', 'angularjs-dropdown-multiselect', 'highcharts-ng', 'angularjs-dropdown-multiselect1', 'angularjs-click', 'file_Upload', 'ngCookies', 'angularjs-expandCollapse', 'blockUI', 'cgNotify', 'ngAutocomplete', 'ui.bootstrap.datetimepicker', 'daterangepicker', 'angularFileUpload', 'ngSanitize', 'dropdown-disabled', 'ng-sortable', 'ngMap', 'angucomplete', 'cfp.hotkeys']);

CRMApp.provider('checkRouting', function () {
    this.$get = function ($window) {
        return "";
    };
});

CRMApp.config(function ($stateProvider, $urlRouterProvider, blockUIConfig, checkRoutingProvider) {
    $urlRouterProvider.otherwise("/Login");
    blockUIConfig.message = '';
    // blockUIConfig.defaults.overlayCSS.opacity = 0;
    //  blockUIConfig.templateUrl = '<b>asdasdsadasdas</b>';
    blockUIConfig.requestFilter = function (config) {
        // If the request starts with '/api/quote' ...
        if (config.url.indexOf("GetLatestUnifiedInboxDataForSelectedProfiles") > 0 ||
            config.url.indexOf("GetAllMessages") > 0 ||
            config.url.indexOf("MarkReadNotificationById") > 0 ||
            config.url.indexOf("GetAllScheduledMessages") > 0 ||
            config.url.indexOf("GetAllQueuedMessages") > 0 ||
            config.url.indexOf("FacebookGetAllPostInfo") > 0 ||
            config.url.indexOf("GetPostDetails") > 0 ||
            config.url.indexOf("GetTweetMinnedReplies") > 0 ||
            config.url.indexOf("GetTweetDirectReplies") > 0 ||
            config.url.indexOf("GetOutlookNewInboxEmails") > 0 ||
            config.url.indexOf("GetOutlookNewSentEmails") > 0 || 
            config.url.indexOf("SetReadStatus") > 0 ||
            config.url.indexOf("GetSelectedProfilesActivity") > 0 ||
            config.url.indexOf("twitterLoadMoreFeeds") > 0 ||
            config.url.indexOf("twitterLoadMoreDMs") > 0 ||
            config.url.indexOf("FacebookLoadMoreFeeds") > 0 ||
            config.url.indexOf("FacebookLoadMoreComments") > 0 ||
            config.url.indexOf("FacebookLoadMoreChats") > 0 ||
            config.url.indexOf("FacebookCommentPost") > 0 ||
            config.url.indexOf("FacebookNewPageChat") > 0 ||
            config.url.indexOf("TwitterReplyTweet") > 0 ||
            config.url.indexOf("TwitterSendDM") > 0 ||
            config.url.indexOf("InsertTaskCompleted") > 0 ||
            config.url.indexOf("GetUnifiedDeckForUserSelectedProfiles") > 0 ||
            config.url.indexOf("GetLatestUnifiedDeckForUserSelectedProfiles") > 0 ||
            config.url.indexOf("GetNextUnifiedDeckForUserSelectedProfilesUnresponded") > 0 ||
            config.url.indexOf("GetUnrespondedQueriesColumnSettings") > 0 ||
            config.url.indexOf("GetSampleRepliesText") > 0 ||
            config.url.indexOf("GetConsumerData") > 0 ||
            config.url.indexOf("TwitterIsFavorited") > 0 ||
            config.url.indexOf("TwitterIsRetweeted") > 0 ||
            config.url.indexOf("TwitterReplyTweet") > 0 ||
            config.url.indexOf("FacebookGetPageChatConversation") > 0 ||
            config.url.indexOf("FacebookIsPostLiked") > 0 ||
            config.url.indexOf("FacebookGetMoreRecentPosts") > 0 ||
            //below lines added on July 05, 2017, on client's request
            config.url.indexOf("GetTweetThreadByTweetId") > 0 ||
            config.url.indexOf("GetTwitterDirectMessages") > 0 ||
            config.url.indexOf("FacebookGetPostInfo") > 0 ||
            config.url.indexOf("FacebookGetUserConversation") > 0 
            //config.url.indexOf("GetUnifiedInboxForSelectedProfiles") > 0  
           ) {//window.location.hash == "#/UnifiedInbox") {
            return false; // ... don't block it.
        }
    };
    var paths = {
        'root': {
            url: "",
            abstract: true,
            views: {
                'header': {
                    templateUrl: base_url + "/Scripts/Views/Layout/Header.html"
                    // controller: 'HeaderCtrl'
                },
                'footer': {
                    templateUrl: base_url + "/Scripts/Views/Layout/Footer.html"
                    //controller: 'FooterCtrl'
                }
            }
            // header:base_url + "/Scripts/Views/Layout/Header.html",
            // templateUrl: base_url + "/Scripts/Views/User/login.html",
            //  controller: "UserController"
        },
        'Login': {
            url: "/Login",
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/User/login.html"
                }
            }

            //  controller: "UserController"
        },
        'Signup': {
            url: "/Signup",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/User/Signup.html"
                },

                'Header@Signup': {
                    templateUrl: base_url + "/Scripts/Views/Layout/Header.html"
                }

            }
        },
        'Signup_Code': {
            url: "/Signup_Code",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/User/Signup_Code.html"
                }

            }
        },
        'CompanyAccountActivation': {
            url: "/CompanyAccountActivation",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/User/CompanyAccountActivation.html"
                }

            }
        },
        'Signup_Twitter': {
            url: "/Signup_Twitter",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/User/Signup_Twitter.html"
                }

            }
        },
        'Account_Activation': {
            url: "/AccountActivation",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/User/AccountActivation.html"
                }

            }
        }/*,
        'Connect_Social': {
            url: "/Connect_Social",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Profile/Connect_Social.html",
                }

            }
        }*/,
        'ResetPassword': {
            url: "/ResetPassword",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/User/ResetPassword.html"
                }

            }
        },
        'SetPassword': {
            url: "/SetPassword",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/User/SetPassword.html"
                }

            }
        },
        'Signup_Package': {
            url: "/Signup_Package",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/User/Signup_Package.html"
                }

            }
        },
        'Signup_BusinessName': {
            url: "/Signup_BusinessName",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/User/Signup_BusinessName.html"
                }

            }
        }
        ,
        /*'Connect_FB_Pages': {
            url: "/Connect_FB_Pages",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Profile/Connect_FB_Pages.html",
                }

            }
        },*/
        'root_NotificationSettings': {
            url: "/NotificationSettings",
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/PersonalProfile/BusinessProfile.html"
                }

                //'Header@BusinessProfile': {
                //    templateUrl: base_url + "/Scripts/Views/Layout/Header.html"
                //},
                //'Footer@BusinessProfile': {
                //    templateUrl: base_url + "/Scripts/Views/Layout/Footer.html"
                //}

            }
        },
        'root_PersonalProfile': {
            url: "/PersonalProfile",
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/PersonalProfile/PersonalProfile.html"
                }
                //'Header@PersonalProfile': {
                //    templateUrl: base_url + "/Scripts/Views/Layout/Header.html"
                //},
                //'Footer@PersonalProfile': {
                //    templateUrl: base_url + "/Scripts/Views/Layout/Footer.html"
                //}

            }
        },

        'root_PatternCustomization': {
            url: "/PatternCustomization",
            params: { comingFrom: "AdminPage" },
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Settings/PatternCustomization.html"
                }
            }
        },

        //Settings
        'root_TagsSettings': {
            url: "/TagsSettings",
            params: { comingFrom: "AdminPage" },
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Settings/TagsSettings.html"
                    //checkRoutingProvider.routeRedirect("PatternCustomization", base_url + "/Scripts/Views/Settings/PatternCustomization.html")
                }
            }
        },
        'root_EmailSettings': {
            url: "/EmailSettings",
            params: { comingFrom: "AdminPage" },
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Settings/EmailSettings.html"
                    //checkRoutingProvider.routeRedirect("PatternCustomization", base_url + "/Scripts/Views/Settings/PatternCustomization.html")
                }
            }
        },
        'root_UserLogs': {
            url: "/UserLogs",
            params: { comingFrom: "AdminPage" },
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/User/UserLogs.html"
                    //checkRoutingProvider.routeRedirect("PatternCustomization", base_url + "/Scripts/Views/Settings/PatternCustomization.html")
                }
            }
        },
        'root_CustomerSettings': {
            url: "/CustomerSettings",
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Settings/TopInfluencersSettings.html"
                    //checkRoutingProvider.routeRedirect("PatternCustomization", base_url + "/Scripts/Views/Settings/PatternCustomization.html")
                }
            }
        },

        'root_TwitterReport': {
            url: "/TwitterReport",
            params: { comingFrom: "AdminPage" },
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Reports/TwitterReport.html"
                }
            }
        },
        'root_FacebookReport': {
            url: "/FacebookReport",
            params: { comingFrom: "AdminPage" },
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Reports/FacebookReport.html"
                }
            }
        },
        'root_GroupManagementReport': {
            url: "/GroupManagementReport",
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Reports/GroupManagementReport.html"
                }
            }
        },
        'root_TaskPerformanceReport': {
            url: "/TaskPerformanceReport",
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Reports/TaskPerformanceReport.html"
                }
            }
        },
        'root_InboundReport': {
            url: "/InboundReport",
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Reports/InboundReport.html"
                }
            }
        },
        'root_ShiftReport': {
            url: "/ShiftReport",
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Reports/ShiftReport.html"
                }
            }
        },
        'root_TeamReport': {
            url: "/AgentPerformanceReport",
            params: { comingFrom: "AdminPage" },
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Reports/TeamReport.html"
                }
            }
        },

        'root_Notifications': {
            url: "/Notifications",
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Notifications/Notifications.html",
                }

                //'Header@PersonalProfile': {
                //    templateUrl: base_url + "/Scripts/Views/Layout/Header.html"
                //},
                //'Footer@PersonalProfile': {
                //    templateUrl: base_url + "/Scripts/Views/Layout/Footer.html"
                //}

            }
        },
        'root_ChangePassword': {
            url: "/ChangePassword",
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/PersonalProfile/ChangePassword.html",
                }

                //'Header@PersonalProfile': {
                //    templateUrl: base_url + "/Scripts/Views/Layout/Header.html"
                //},
                //'Footer@PersonalProfile': {
                //    templateUrl: base_url + "/Scripts/Views/Layout/Footer.html"
                //}

            }
        },
        'root_Dashboard': {
            url: "/Dashboard",
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Dashboard/dashboard.html"
                }
            }
        },
        'root_GroupAndTeam': {
            url: "/GroupAndTeam",
            params: { comingFrom: "AdminPage" },
            //views: {
            //    '': {
            //        templateUrl: base_url + "/Scripts/Views/Settings/GroupSettings.html",
            //    }
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Settings/GroupAndTeam.html"
                }
                //'Header@GroupSettings': {
                //    templateUrl: base_url + "/Scripts/Views/Layout/Header.html"
                //},
                //'Footer@GroupSettings': {
                //    templateUrl: base_url + "/Scripts/Views/Layout/Footer.html"
                //}

            }
        },
        'root_GroupAndTeam_groupAdd':
            {
                url: '/groupAdd',
                //parent: 'root.GroupSettings',
                // trigger the modal to open when this route is active
                onEnter: ['$stateParams', '$state', '$uibModal', '$rootScope',
                  function ($stateParams, $state, $uibModal, $rootScope) {
                      $uibModal

                        // handle modal open
                        .open({
                            animation: true,
                            templateUrl: '/Scripts/Views/Shared/_GroupAdd.html',
                            //controller: ['$scope', '$uibModalInstance',
                            //  function ($scope, $uibModalInstance) {
                            //      // handle after clicking Cancel button
                            //      $scope.cancel = function() {
                            //          $scope.$dismiss();
                            //      };
                            //      // close modal after clicking OK button
                            //      $scope.ok = function() {
                            //          $scope.$close(true);
                            //      };
                            //  }
                            //]
                            controller: '_GroupAddController'
                        })

                        // change route after modal result
                        .result.then(function () {

                            // change route after clicking OK button
                            $state.transitionTo('root.GroupAndTeam');
                        }, function () {
                            $rootScope.previousPage = $rootScope.previousPage.replace("/", "");
                            // alert($rootScope.previousPage);
                            $rootScope.previousPage = $rootScope.previousPage.split('/')[0]
                            // change route after clicking Cancel button or clicking background
                            $state.transitionTo('root.' + $rootScope.previousPage);
                        });

                  }
                ]

            },
        'root_GroupAndTeam_ExistingProfile':
            {
                url: '/ExistingProfile',
                //parent: 'root.GroupSettings',
                // trigger the modal to open when this route is active
                onEnter: ['$stateParams', '$state', '$uibModal', '$rootScope',
                  function ($stateParams, $state, $uibModal, $rootScope) {
                      $uibModal

                        // handle modal open
                        .open({
                            animation: true,
                            templateUrl: '/Scripts/Views/Shared/_ExistingProfile.html',
                            //controller: ['$scope', '$uibModalInstance',
                            //  function ($scope, $uibModalInstance) {
                            //      // handle after clicking Cancel button
                            //      $scope.cancel = function() {
                            //          $scope.$dismiss();
                            //      };
                            //      // close modal after clicking OK button
                            //      $scope.ok = function() {
                            //          $scope.$close(true);
                            //      };
                            //  }
                            //]
                            controller: '_ExistingProfileController'
                        })

                        // change route after modal result
                        .result.then(function () {

                            // change route after clicking OK button
                            $state.transitionTo('root.GroupAndTeam');
                        }, function () {
                            $rootScope.previousPage = $rootScope.previousPage.replace("/", "");
                            // alert($rootScope.previousPage);
                            $rootScope.previousPage = $rootScope.previousPage.split('/')[0]
                            // change route after clicking Cancel button or clicking background
                            $state.transitionTo('root.' + $rootScope.previousPage);
                        });

                  }
                ]

            },
        'root_GroupAndTeam_ExistingMember':
            {
                url: '/ExistingMember',
                //parent: 'root.GroupSettings',
                // trigger the modal to open when this route is active
                onEnter: ['$stateParams', '$state', '$uibModal', '$rootScope',
                  function ($stateParams, $state, $uibModal, $rootScope) {
                      $uibModal

                        // handle modal open
                        .open({
                            animation: true,
                            templateUrl: '/Scripts/Views/Shared/_ExistingMember.html',
                            //controller: ['$scope', '$uibModalInstance',
                            //  function ($scope, $uibModalInstance) {
                            //      // handle after clicking Cancel button
                            //      $scope.cancel = function() {
                            //          $scope.$dismiss();
                            //      };
                            //      // close modal after clicking OK button
                            //      $scope.ok = function() {
                            //          $scope.$close(true);
                            //      };
                            //  }
                            //]
                            controller: '_ExistingMemberController'
                        })

                        // change route after modal result
                        .result.then(function () {

                            // change route after clicking OK button
                            $state.transitionTo('root.GroupAndTeam');
                        }, function () {
                            $rootScope.previousPage = $rootScope.previousPage.replace("/", "");
                            // alert($rootScope.previousPage);
                            $rootScope.previousPage = $rootScope.previousPage.split('/')[0]
                            // change route after clicking Cancel button or clicking background
                            $state.transitionTo('root.' + $rootScope.previousPage);
                        });

                  }
                ]

            },
        'root_GroupAndTeam_profileAdd':
           {
               url: '/profileAdd',
               parent: 'root.GroupAndTeam',
               // trigger the modal to open when this route is active
               onEnter: ['$stateParams', '$state', '$uibModal', '$rootScope',
                 function ($stateParams, $state, $uibModal, $rootScope) {
                     $uibModal

                       // handle modal open
                       .open({
                           animation: true,
                           templateUrl: '/Scripts/Views/Shared/_ProfileAdd.html',

                           controller: '_ProfileAddController'
                       })

                       // change route after modal result
                       .result.then(function (obj) {
                           if (obj == 'undefined') {
                               // change route after clicking OK button
                               $state.transitionTo('root.GroupAndTeam');
                           }
                       }, function (obj) {
                           if (obj == 'undefined') {
                               $rootScope.previousPage = $rootScope.previousPage.replace("/", "");
                               $rootScope.previousPage = $rootScope.previousPage.split('/')[0]
                               // change route after clicking Cancel button or clicking background
                               $state.transitionTo('root.' + $rootScope.previousPage);
                           }
                       });

                 }
               ]

           },
        'root_ConnectOutlook':
          {
              url: '/OutlookConnect',
              parent: 'root.GroupAndTeam',
              // trigger the modal to open when this route is active
              onEnter: ['$stateParams', '$state', '$uibModal', '$rootScope', 
                function ($stateParams, $state, $uibModal, $rootScope) {
                    $uibModal

                      // handle modal open
                      .open({
                          animation: true,
                          templateUrl: '/Scripts/Views/Shared/_OutlookConnect.html',

                          controller: '_OutlookConnectController'
                      })

                      // change route after modal result
                      .result.then(function () {

                          // change route after clicking OK button
                         // $state.transitionTo('root.GroupAndTeam');
                      }, function () {
                          $rootScope.previousPage = $rootScope.previousPage.replace("/", "");
                          $rootScope.previousPage = $rootScope.previousPage.split('/')[0]
                          // change route after clicking Cancel button or clicking background
                          $state.transitionTo('root.' + $rootScope.previousPage);
                      });

                }
              ]

          },
        'root_GroupAndTeam_pagesAdd':
          {
              url: '/pagesAdd',
              parent: 'root.GroupAndTeam',
              // trigger the modal to open when this route is active
              onEnter: ['$stateParams', '$state', '$uibModal', '$rootScope',
                function ($stateParams, $state, $uibModal, $rootScope) {
                    $uibModal

                      // handle modal open
                      .open({
                          animation: true,
                          templateUrl: '/Scripts/Views/Shared/_PagesAdd.html',

                          controller: '_PagesAddController'
                      })

                      // change route after modal result
                      .result.then(function () {
                          // change route after clicking OK button
                          $state.transitionTo('root.GroupAndTeam');
                      }, function () {
                          $rootScope.previousPage = $rootScope.previousPage.replace("/", "");
                          $rootScope.previousPage = $rootScope.previousPage.split('/')[0]
                          // change route after clicking Cancel button or clicking background
                          $state.transitionTo('root.' + $rootScope.previousPage);
                      });

                }
              ]

          },
        'root_GroupAndTeam_userAdd':
          {
              url: '/userAdd',
              parent: 'root.GroupAndTeam',
              // trigger the modal to open when this route is active
              onEnter: ['$stateParams', '$state', '$uibModal', '$rootScope',
                function ($stateParams, $state, $uibModal, $rootScope) {
                    $uibModal

                      // handle modal open
                      .open({
                          animation: true,
                          templateUrl: '/Scripts/Views/Shared/_UserAdd.html',

                          controller: '_UserAddController'
                      })

                      // change route after modal result
                      .result.then(function () {
                          // change route after clicking OK button
                          $state.transitionTo('root.GroupAndTeam');
                      }, function () {
                          $rootScope.previousPage = $rootScope.previousPage.replace("/", "");
                          $rootScope.previousPage = $rootScope.previousPage.split('/')[0]
                          // change route after clicking Cancel button or clicking background
                          $state.transitionTo('root.' + $rootScope.previousPage);
                      });

                }
              ]

          },
        'root_GroupAndTeam_GroupsUserAdd':
          {
              url: '/GroupsUserAdd',
              parent: 'root.GroupAndTeam',
              // trigger the modal to open when this route is active
              onEnter: ['$stateParams', '$state', '$uibModal',
                function ($stateParams, $state, $uibModal) {
                    $uibModal

                      // handle modal open
                      .open({
                          animation: true,
                          templateUrl: '/Scripts/Views/Shared/_GroupsUserAdd.html',

                          controller: '_GroupsUserAddController'
                      })

                      // change route after modal result
                      .result.then(function () {
                          // change route after clicking OK button
                          $state.transitionTo('root.GroupAndTeam');
                      }, function () {
                          // change route after clicking Cancel button or clicking background
                          $state.transitionTo('root.GroupAndTeam');
                      });

                }
              ]

          },
        'root_GroupSettings': {
            url: "/GroupSettings",
            //views: {
            //    '': {
            //        templateUrl: base_url + "/Scripts/Views/Settings/GroupSettings.html",
            //    }
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Settings/GroupSettings.html"
                }
                //'Header@GroupSettings': {
                //    templateUrl: base_url + "/Scripts/Views/Layout/Header.html"
                //},
                //'Footer@GroupSettings': {
                //    templateUrl: base_url + "/Scripts/Views/Layout/Footer.html"
                //}

            }
        },
        'root_GroupSettings_groupAdd':
            {
                url: '/groupAdd',
                //parent: 'root.GroupSettings',
                // trigger the modal to open when this route is active
                onEnter: ['$stateParams', '$state', '$uibModal', '$rootScope',
                  function ($stateParams, $state, $uibModal, $rootScope) {
                      $uibModal

                        // handle modal open
                        .open({
                            animation: true,
                            templateUrl: '/Scripts/Views/Shared/_GroupAdd.html',
                            //controller: ['$scope', '$uibModalInstance',
                            //  function ($scope, $uibModalInstance) {
                            //      // handle after clicking Cancel button
                            //      $scope.cancel = function() {
                            //          $scope.$dismiss();
                            //      };
                            //      // close modal after clicking OK button
                            //      $scope.ok = function() {
                            //          $scope.$close(true);
                            //      };
                            //  }
                            //]
                            controller: '_GroupAddController'
                        })

                        // change route after modal result
                        .result.then(function () {

                            // change route after clicking OK button
                            $state.transitionTo('root.GroupSettings');
                        }, function () {
                            $rootScope.previousPage = $rootScope.previousPage.replace("/", "");
                            // alert($rootScope.previousPage);
                            $rootScope.previousPage = $rootScope.previousPage.split('/')[0]
                            // change route after clicking Cancel button or clicking background
                            $state.transitionTo('root.' + $rootScope.previousPage);
                        });

                  }
                ]

            },
        'root_GroupSettings_ExistingProfile':
            {
                url: '/ExistingProfile',
                //parent: 'root.GroupSettings',
                // trigger the modal to open when this route is active
                onEnter: ['$stateParams', '$state', '$uibModal', '$rootScope',
                  function ($stateParams, $state, $uibModal, $rootScope) {
                      $uibModal

                        // handle modal open
                        .open({
                            animation: true,
                            templateUrl: '/Scripts/Views/Shared/_ExistingProfile.html',
                            //controller: ['$scope', '$uibModalInstance',
                            //  function ($scope, $uibModalInstance) {
                            //      // handle after clicking Cancel button
                            //      $scope.cancel = function() {
                            //          $scope.$dismiss();
                            //      };
                            //      // close modal after clicking OK button
                            //      $scope.ok = function() {
                            //          $scope.$close(true);
                            //      };
                            //  }
                            //]
                            controller: '_ExistingProfileController'
                        })

                        // change route after modal result
                        .result.then(function () {

                            // change route after clicking OK button
                            $state.transitionTo('root.GroupSettings');
                        }, function () {
                            $rootScope.previousPage = $rootScope.previousPage.replace("/", "");
                            // alert($rootScope.previousPage);
                            $rootScope.previousPage = $rootScope.previousPage.split('/')[0]
                            // change route after clicking Cancel button or clicking background
                            $state.transitionTo('root.' + $rootScope.previousPage);
                        });

                  }
                ]

            },
        'root_GroupSettings_ExistingMember':
            {
                url: '/ExistingMember',
                //parent: 'root.GroupSettings',
                // trigger the modal to open when this route is active
                onEnter: ['$stateParams', '$state', '$uibModal', '$rootScope',
                  function ($stateParams, $state, $uibModal, $rootScope) {
                      $uibModal

                        // handle modal open
                        .open({
                            animation: true,
                            templateUrl: '/Scripts/Views/Shared/_ExistingMember.html',
                            //controller: ['$scope', '$uibModalInstance',
                            //  function ($scope, $uibModalInstance) {
                            //      // handle after clicking Cancel button
                            //      $scope.cancel = function() {
                            //          $scope.$dismiss();
                            //      };
                            //      // close modal after clicking OK button
                            //      $scope.ok = function() {
                            //          $scope.$close(true);
                            //      };
                            //  }
                            //]
                            controller: '_ExistingMemberController'
                        })

                        // change route after modal result
                        .result.then(function () {

                            // change route after clicking OK button
                            $state.transitionTo('root.GroupSettings');
                        }, function () {
                            $rootScope.previousPage = $rootScope.previousPage.replace("/", "");
                            // alert($rootScope.previousPage);
                            $rootScope.previousPage = $rootScope.previousPage.split('/')[0]
                            // change route after clicking Cancel button or clicking background
                            $state.transitionTo('root.' + $rootScope.previousPage);
                        });

                  }
                ]

            },
        'root_GroupSettings_profileAdd':
           {
               url: '/profileAdd',
               parent: 'root.GroupSettings',
               // trigger the modal to open when this route is active
               onEnter: ['$stateParams', '$state', '$uibModal', '$rootScope',
                 function ($stateParams, $state, $uibModal, $rootScope) {
                     $uibModal

                       // handle modal open
                       .open({
                           animation: true,
                           templateUrl: '/Scripts/Views/Shared/_ProfileAdd.html',

                           controller: '_ProfileAddController'
                       })

                       // change route after modal result
                       .result.then(function () {

                           // change route after clicking OK button
                           $state.transitionTo('root.GroupSettings');
                       }, function () {
                           $rootScope.previousPage = $rootScope.previousPage.replace("/", "");
                           $rootScope.previousPage = $rootScope.previousPage.split('/')[0]
                           // change route after clicking Cancel button or clicking background
                           $state.transitionTo('root.' + $rootScope.previousPage);
                       });

                 }
               ]

           },
        'root_GroupSettings_pagesAdd':
          {
              url: '/pagesAdd',
              parent: 'root.GroupSettings',
              // trigger the modal to open when this route is active
              onEnter: ['$stateParams', '$state', '$uibModal', '$rootScope',
                function ($stateParams, $state, $uibModal, $rootScope) {
                    $uibModal

                      // handle modal open
                      .open({
                          animation: true,
                          templateUrl: '/Scripts/Views/Shared/_PagesAdd.html',

                          controller: '_PagesAddController'
                      })

                      // change route after modal result
                      .result.then(function () {
                          // change route after clicking OK button
                          $state.transitionTo('root.GroupSettings');
                      }, function () {
                          $rootScope.previousPage = $rootScope.previousPage.replace("/", "");
                          $rootScope.previousPage = $rootScope.previousPage.split('/')[0]
                          // change route after clicking Cancel button or clicking background
                          $state.transitionTo('root.' + $rootScope.previousPage);
                      });

                }
              ]

          },
        'root_GroupSettings_userAdd':
          {
              url: '/userAdd',
              parent: 'root.GroupSettings',
              // trigger the modal to open when this route is active
              onEnter: ['$stateParams', '$state', '$uibModal', '$rootScope',
                function ($stateParams, $state, $uibModal, $rootScope) {
                    $uibModal

                      // handle modal open
                      .open({
                          animation: true,
                          templateUrl: '/Scripts/Views/Shared/_UserAdd.html',

                          controller: '_UserAddController'
                      })

                      // change route after modal result
                      .result.then(function () {
                          // change route after clicking OK button
                          $state.transitionTo('root.GroupSettings');
                      }, function () {
                          $rootScope.previousPage = $rootScope.previousPage.replace("/", "");
                          $rootScope.previousPage = $rootScope.previousPage.split('/')[0]
                          // change route after clicking Cancel button or clicking background
                          $state.transitionTo('root.' + $rootScope.previousPage);
                      });

                }
              ]

          },
        'root_AnalyticReport': {
            url: "/AnalyticReport",
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Analytics/AnalyticsReport.html"
                }

                //'Header@AnalyticReport': {
                //    templateUrl: base_url + "/Scripts/Views/Layout/Header.html"
                //},
                //'Footer@AnalyticReport': {
                //    templateUrl: base_url + "/Scripts/Views/Layout/Footer.html"
                //}

            }
        },
        'root_InstagramReport': {
            url: "/InstagramReport",
            params: { comingFrom: "AdminPage" },
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Report/InstagramReport.html"
                }
            }
        }
        /*,
        'root_UserNSocialProfile': {
            url: "/UserNSocialProfile",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/PersonalProfile/UserNSocialProfile.html",
                }

                //'Header@UserNSocialProfile': {
                //    templateUrl: base_url + "/Scripts/Views/Layout/Header.html"
                //},
                //'Footer@UserNSocialProfile': {
                //    templateUrl: base_url + "/Scripts/Views/Layout/Footer.html"
                //}

            }
        },
        'root_UserNSocialProfile_GroupsUserAdd':
          {
              url: '/GroupsUserAdd',
              parent: 'root.UserNSocialProfile',
              // trigger the modal to open when this route is active
              onEnter: ['$stateParams', '$state', '$uibModal',
                function ($stateParams, $state, $uibModal) {
                    $uibModal

                      // handle modal open
                      .open({
                          animation: true,
                          templateUrl: '/Scripts/Views/Shared/_GroupsUserAdd.html',

                          controller: '_GroupsUserAddController'
                      })

                      // change route after modal result
                      .result.then(function () {
                          // change route after clicking OK button
                          $state.transitionTo('root.UserNSocialProfile');
                      }, function () {
                          // change route after clicking Cancel button or clicking background
                          $state.transitionTo('root.UserNSocialProfile');
                      });

                }
              ]

          }*/,
        'root_UnifiedInbox': {
            url: "/UnifiedInbox",
            params: { pageType: "Unified" },

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/UnifiedInbox/unifiedinbox.html"
                }

                //'Header@UnifiedInbox': {
                //    templateUrl: base_url + "/Scripts/Views/Layout/Header.html"
                //},
                //'Footer@UnifiedInbox': {
                //    templateUrl: base_url + "/Scripts/Views/Layout/Footer.html"
                //}

            }
        },
        'root_UnifiedDeck': {
            url: "/Responder",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/UnifiedDeck/unifiedDeck.html"
                }
            }
        },
        'root_Outlook': {
            url: "/Outlook",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/UnifiedInbox/Outlook.html"
                }

                //'Header@UnifiedInbox': {
                //    templateUrl: base_url + "/Scripts/Views/Layout/Header.html"
                //},
                //'Footer@UnifiedInbox': {
                //    templateUrl: base_url + "/Scripts/Views/Layout/Footer.html"
                //}

            }
        },
        'root_ErrorPage': {
            url: "/ErrorPage",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/User/ErrorPage.html",
                }

            }

        },
        'root_UnifiedContact': {
            url: "/UnifiedContact",
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/UnifiedInbox/unifiedContact.html"
                }
            }
        },
        'root_ArchiveInbox': {
            url: "/ArchiveInbox",
            params: { pageType: "Archive" },

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/UnifiedInbox/unifiedinbox.html"
                }

                //'Header@UnifiedInbox': {
                //    templateUrl: base_url + "/Scripts/Views/Layout/Header.html"
                //},
                //'Footer@UnifiedInbox': {
                //    templateUrl: base_url + "/Scripts/Views/Layout/Footer.html"
                //}

            }
        },
        'root_CompletedInbox': {
            url: "/Completed",
            params: { pageType: "Completed" },

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/UnifiedInbox/unifiedinbox.html"
                }
            }
        },
        'root_ArchiveContact': {
            url: "/ArchiveContact",
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/UnifiedInbox/ArchiveContact.html"
                }

                //'Header@UnifiedInbox': {
                //    templateUrl: base_url + "/Scripts/Views/Layout/Header.html"
                //},
                //'Footer@UnifiedInbox': {
                //    templateUrl: base_url + "/Scripts/Views/Layout/Footer.html"
                //}

            }
        },
        'root_CreateTask': {
            url: "/CreateCase",
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Task/CreateTask.html"
                }
            }
        },
        'root_CustomerProfiles': {
            url: "/CustomerProfiles",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/CustomerProfile/CustomerProfile.html"
                }
            }
        },
        'root_BannedAndWarnedUser': {
            url: "/BannedAndWarnedUser",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/CustomerProfile/WarnBanUsers.html"
                }
            }
        },
        'root_BannedAndWarnedReport': {
            url: "/BannedAndWarnedReport",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/CustomerProfile/WarnBanUsersReport.html"
                }
            }
        },
        'root_AllTask': {
            url: "/AllCase",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Task/TaskList.html"
                }
            }
        },

        'root_DraftTask': {
            url: "/DraftCase",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Task/DraftTask.html"
                }
            }
        },

        'root_Profiles': {
            url: "/Profiles",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Task/Profiles.html"
                }
            }
        },

        'root_ClosedTask': {
            url: "/ClosedCase",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Task/TaskList.html"
                }
            }
        },

        'root_SocialFeedsRSS': {
            url: "/SocialFeedsRSS",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Feeds/SocialFeedsRSS.html"
                }
            }
        },
        'root_RSSFeedsPages': {
            url: "/RSSFeedsPages",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Feeds/SocialFeedsRSS.html"
                }

            }
        },
        'root_RSSFeedsKeywords': {
            url: "/RSSFeedsKeywords",
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Feeds/SocialFeedsRSS.html"
                }
            }
        },

        'root_SocialFeedsFacebook': {
            url: "/SocialFeedsFacebook",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Feeds/SocialFeedsFacebook.html"
                }
            }
        },

        'root_SocialFeedsTwitter': {
            url: "/SocialFeedsTwitter",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Feeds/SocialFeedsTwitter.html"
                }
            }
        },
        'root_SocialFeedsInstagram': {
            url: "/SocialFeedsInstagram",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Feeds/SocialFeedsInstagram.html"
                }
            }
        },
        'root_SocialFeedsLinkedIn': {
            url: "/SocialFeedsLinkedIn",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Feeds/SocialFeedsLinkedIn.html"
                }
            }
        },

        'root_SocialFeedsRSSFeedly': {
            url: "/SocialFeedsRSSFeedly",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Feeds/SocialFeedsRSSFeedly.html"
                }
            }
        },
        'root_QueuedMessages': {
            url: "/QueuedMessages",
            params: { comingFrom: "QueuePage" },
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Publisher/QueuedMessages.html"
                }
            }
        },
        'root_Drafts': {
            url: "/Drafts",
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Publisher/Drafts.html"
                }
            }
        },
        'root_PendingApproval': {
            url: "/PendingApproval",
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Publisher/PendingApproval.html"
                }
            }
        },
        'root_RejectedMessages': {
            url: "/RejectedMessages",
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Publisher/RejectedMessages.html"
                }
            }
        },
        'root_Schedular': {
            url: "/Schedular",
            params: { comingFrom: "SchedularPage" },
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Publisher/Schedular.html"
                }
            }
        },
        'root_ScheduledMessages': {
            url: "/ScheduledMessages",
            params: { comingFrom: "SchedulePage" },
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Publisher/ScheduledMessages.html"
                }
            }
        },
        'root_SentMessages': {
            url: "/SentMessages",
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Publisher/SentMessages.html"
                }
            }
        },
        'root_ScheduledQueueList': {
            url: "/ScheduledQueueList",
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Publisher/ScheduledQueueList.html"
                }
            }
        },
        'adminRoot': {
            url: "",
            abstract: true,
            views: {
                'header': {
                    templateUrl: base_url + "/Scripts/Views/Layout/AdminHeader.html"
                    // controller: 'HeaderCtrl'
                },
                'footer': {
                    templateUrl: base_url + "/Scripts/Views/Layout/Footer.html"
                    //controller: 'FooterCtrl'
                }
            }
            // header:base_url + "/Scripts/Views/Layout/Header.html",
            // templateUrl: base_url + "/Scripts/Views/User/login.html",
            //  controller: "UserController"
        },
        'adminRoot_Packages': {
            url: "/MembershipPackages",
            params: { comingFrom: "Admin" },
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Admin/Packages.html"
                }
            }
        },
        'adminRoot_ErrorPage': {
            url: "/MembershipErrorPage",

            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/User/ErrorPage.html",
                }
            }

        },
        'adminRoot_DashboardView': {
            url: "/DashboardView",
            params: { comingFrom: "Admin" },
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Admin/DashboardView.html"
                }
            }
        },
        'adminRoot_Companies': {
            url: "/Companies",
            params: { comingFrom: "Admin" },
            views: {
                'container@': {
                    templateUrl: base_url + "/Scripts/Views/Admin/Companies.html"
                }
            }
        }
    }

    $stateProvider.state('root', paths.root);
    $stateProvider.state('Login', paths.Login);
    $stateProvider.state('Signup', paths.Signup);
    $stateProvider.state('Signup_Code', paths.Signup_Code);
    $stateProvider.state('CompanyAccountActivation', paths.CompanyAccountActivation);
    $stateProvider.state('Signup_Twitter', paths.Signup_Twitter);
    //$stateProvider.state('Connect_Social', paths.Connect_Social);
    $stateProvider.state('ResetPassword', paths.ResetPassword);
    $stateProvider.state('SetPassword', paths.SetPassword);
    $stateProvider.state('Signup_Package', paths.Signup_Package);
    $stateProvider.state('Account_Activation', paths.Account_Activation);
    //$stateProvider.state('GroupSettings._GroupAdd', paths.GroupSettings._GroupAdd);

    $stateProvider.state('Signup_BusinessName', paths.Signup_BusinessName);
    //$stateProvider.state('Connect_FB_Pages', paths.Connect_FB_Pages);
    $stateProvider.state('root.NotificationSettings', paths.root_NotificationSettings);
    $stateProvider.state('root.PersonalProfile', paths.root_PersonalProfile);
    $stateProvider.state('root.PatternCustomization', paths.root_PatternCustomization);
    //Settings
    $stateProvider.state('root.TagsSettings', paths.root_TagsSettings);
    $stateProvider.state('root.EmailSettings', paths.root_EmailSettings);
    $stateProvider.state('root.CustomerSettings', paths.root_CustomerSettings);
    $stateProvider.state('root.UserLogs', paths.root_UserLogs);
    $stateProvider.state('root.ChangePassword', paths.root_ChangePassword);
    $stateProvider.state('root.Notifications', paths.root_Notifications);
    $stateProvider.state('root.TwitterReport', paths.root_TwitterReport);
    $stateProvider.state('root.FacebookReport', paths.root_FacebookReport);
    //$stateProvider.state('root.GroupManagementReport', paths.root_GroupManagementReport);
    //$stateProvider.state('root.TaskPerformanceReport', paths.root_TaskPerformanceReport);
    $stateProvider.state('root.TeamReport', paths.root_TeamReport);
    //$stateProvider.state('root.AnalyticReport', paths.root_AnalyticReport);
    $stateProvider.state('root.InstagramReport', paths.root_InstagramReport);
    $stateProvider.state('root.InboundReport', paths.root_InboundReport);
    $stateProvider.state('root.ShiftReport', paths.root_ShiftReport);
    //$stateProvider.state('root.UserNSocialProfile', paths.root_UserNSocialProfile);
    //$stateProvider.state('root.Dashboard', paths.root_Dashboard);

    //$stateProvider.state('root.UserNSocialProfile.GroupsUserAdd', paths.root_UserNSocialProfile_GroupsUserAdd);

   // $stateProvider.state('root.GroupSettings', paths.root_GroupSettings);

    $stateProvider.state('root.GroupAndTeam', paths.root_GroupAndTeam);
    $stateProvider.state('root.GroupAndTeam.groupAdd', paths.root_GroupAndTeam_groupAdd);
    $stateProvider.state('root.GroupAndTeam.ExistingProfile', paths.root_GroupAndTeam_ExistingProfile);
    $stateProvider.state('root.GroupAndTeam.ExistingMember', paths.root_GroupAndTeam_ExistingMember);
    $stateProvider.state('root.GroupAndTeam.profileAdd', paths.root_GroupAndTeam_profileAdd);
    //$stateProvider.state('root.ConnectOutlook', paths.root_ConnectOutlook);
    $stateProvider.state('root.GroupAndTeam.pagesAdd', paths.root_GroupAndTeam_pagesAdd);
    $stateProvider.state('root.GroupAndTeam.userAdd', paths.root_GroupAndTeam_userAdd);
    $stateProvider.state('root.GroupAndTeam.GroupsUserAdd', paths.root_GroupAndTeam_GroupsUserAdd);
    //GroupSettings_groupAdd GroupSettings_profileAdd
    //$stateProvider.state('root.GroupSettings.groupAdd', paths.root_GroupSettings_groupAdd);
    //$stateProvider.state('root.GroupSettings.ExistingProfile', paths.root_GroupSettings_ExistingProfile);
    //$stateProvider.state('root.GroupSettings.ExistingMember', paths.root_GroupSettings_ExistingMember);
    //$stateProvider.state('root.GroupSettings.profileAdd', paths.root_GroupSettings_profileAdd);
    //$stateProvider.state('root.GroupSettings.pagesAdd', paths.root_GroupSettings_pagesAdd);
    //$stateProvider.state('root.GroupSettings.userAdd', paths.root_GroupSettings_userAdd);
    $stateProvider.state('root.UnifiedInbox', paths.root_UnifiedInbox);
    $stateProvider.state('root.CustomerProfiles', paths.root_CustomerProfiles);
    $stateProvider.state('root.BannedAndWarnedUser', paths.root_BannedAndWarnedUser);
    $stateProvider.state('root.BannedAndWarnedReport', paths.root_BannedAndWarnedReport);
    $stateProvider.state('root.CompletedInbox', paths.root_CompletedInbox);
    $stateProvider.state('root.UnifiedDeck', paths.root_UnifiedDeck);
    //$stateProvider.state('root.Outlook', paths.root_Outlook);
    $stateProvider.state('root.ErrorPage', paths.root_ErrorPage);
    $stateProvider.state('root.UnifiedContact', paths.root_UnifiedContact);
    $stateProvider.state('root.ArchiveInbox', paths.root_ArchiveInbox);
    //$stateProvider.state('root.ArchiveContact', paths.root_ArchiveContact);
    //$stateProvider.state('root.CreateTask', paths.root_CreateTask);
    //$stateProvider.state('root.AllTask', paths.root_AllTask);
    //$stateProvider.state('root.DraftTask', paths.root_DraftTask);
    //$stateProvider.state('root.Profiles', paths.root_Profiles);
    //$stateProvider.state('root.ClosedTask', paths.root_ClosedTask);
    //$stateProvider.state('root.SocialFeedsRSS', paths.root_SocialFeedsRSS);
    //$stateProvider.state('root.SocialFeedsInstagram', paths.root_SocialFeedsInstagram);
    //$stateProvider.state('root.SocialFeedsFacebook', paths.root_SocialFeedsFacebook);
    //$stateProvider.state('root.SocialFeedsTwitter', paths.root_SocialFeedsTwitter);
    //$stateProvider.state('root.SocialFeedsLinkedIn', paths.root_SocialFeedsLinkedIn);
    //$stateProvider.state('root.SocialFeedsRSSFeedly', paths.root_SocialFeedsRSSFeedly);
    //$stateProvider.state('root.RSSFeedsPages', paths.root_RSSFeedsPages);
    //$stateProvider.state('root.RSSFeedsKeywords', paths.root_RSSFeedsKeywords);
    $stateProvider.state('root.QueuedMessages', paths.root_QueuedMessages);
    //$stateProvider.state('root.Drafts', paths.root_Drafts);
    //$stateProvider.state('root.PendingApproval', paths.root_PendingApproval);
    //$stateProvider.state('root.RejectedMessages', paths.root_RejectedMessages);
    $stateProvider.state('root.Schedular', paths.root_Schedular);
    $stateProvider.state('root.ScheduledMessages', paths.root_ScheduledMessages);
    //$stateProvider.state('root.SentMessages', paths.root_SentMessages);
    //$stateProvider.state('root.ScheduledQueueList', paths.root_ScheduledQueueList);

    //$stateProvider.state('adminRoot', paths.adminRoot);
    //$stateProvider.state('adminRoot.Packages', paths.adminRoot_Packages);
    //$stateProvider.state('adminRoot.ErrorPage', paths.adminRoot_ErrorPage);
    //$stateProvider.state('adminRoot.DashboardView', paths.adminRoot_DashboardView);
    //$stateProvider.state('adminRoot.Companies', paths.adminRoot_Companies);

    //$stateProvider.state('app1', {
    //    url: '/app1',
    //    parent: 'GroupSettings.profileAdd',
    //    views: {
    //        'app1@': {
    //            template: '<p>sign-in</p>'
    //        }
    //    }
    //})
    //  $stateProvider.state('GroupSettings.profileAdd.app1', { url: '/app1', templateUrl: 'app1.html' });
    //comment
});

// make back button handle closing the modal
CRMApp.run(['$rootScope', '$uibModalStack', '$location', '$uibModal', '$window', '$cookies', 'AuthService',
    function ($rootScope, $uibModalStack, $location, $uibModal, $window, $cookies, AuthService) {

      $rootScope.$on('$stateChangeStart', function () {
          var top = $uibModalStack.getTop();
          if (top) {
              $uibModalStack.dismiss(top.key);
          }
      });
      $rootScope.$on('$locationChangeSuccess', function (e, newLocation, oldLocation) {
          $('#appBody').css("overflow", "auto");

          var userObject = $cookies.getObject("userSessionCRM");
          var newLocationArr =[];
          if (newLocation != undefined && newLocation != null && newLocation.indexOf('#') !== -1) {
              newLocationArr = newLocation.split('#')[1].split('/');
          }
          else {
              newLocationArr.push("");
            }
          
          if (newLocationArr != undefined && newLocationArr!=null&& newLocationArr.length == 1) {
              $location.path("/Login");
          }
          else {
              if (userObject == null || userObject == undefined || userObject.UserId == undefined || userObject.UserId == "" || userObject.UserId == 0) {
                  if (newLocationArr[1].indexOf("SetPassword") == -1 && newLocationArr[1].indexOf("ResetPassword") == -1 && newLocationArr[1].indexOf("Login") == -1)
                      $location.path("/Login");
              }
              else {
                  AuthService.VerifyCookies().then(function (l) {
                      if (l.data == false) {
                          $rootScope.selectedGroupId = null;
                          $rootScope.selectedCompanyId = null;
                          $rootScope.selectedCompanyName = null;
                          $rootScope.currentUser = null;
                          $rootScope.FirstCompanyName = null;
                          $rootScope.FirstCompanyId = null;
                          $rootScope.FirstGroupId = null;
                          $rootScope.IsAdmin = null;
                          $rootScope.IsPersonal = null;
                          $rootScope.PostRespond = null;
                          $rootScope.PostEdit = null;
                          $rootScope.PostSchedule = null;
                          $rootScope.PostQueue = null;
                          $rootScope.PostApproval = null;
                          $rootScope.TwitTweets = null;
                          $rootScope.TwitDMs = null;
                          $rootScope.FBPosts = null;
                          $rootScope.FBComs = null;
                          $rootScope.FBChats = null;
                          $rootScope.IMPosts = null;
                          $rootScope.IMComs = null;

                          $cookies.remove("userSessionCRM", { path: '/' });
                          $location.path("/Login");
                      }
                  });

                  $rootScope.previousPage = oldLocation.split('#')[1];
                  $rootScope.currentUser = JSON.stringify(userObject);
                  $rootScope.IsAdmin = userObject.IsAdmin;
                  $rootScope.PackageName = userObject.PackageName;
                  $rootScope.FirstCompanyName = userObject.CompanyName;
                  $rootScope.FirstCompanyId = userObject.CompanyId;
                  $rootScope.FirstGroupId = userObject.GroupId;

                  var Rights = jQuery.parseJSON($rootScope.currentUser).AccessRights;
                  var Reports = jQuery.parseJSON($rootScope.currentUser).AccessReports;
                  $rootScope.AssignRights(Rights, Reports);

                  if (newLocationArr[1].indexOf("Login") !== -1) {
                      $location.path("/Responder");
                  }
                  else {
                      $("title").html("360 CRM | " + newLocationArr[1]);
                      if (
                          ((
                                newLocationArr[1].indexOf("InstagramReport") !== -1 ||
                                newLocationArr[1].indexOf("GroupAndTeam") !== -1 ||
                               // newLocationArr[1].indexOf("AgentPerformanceReport") !== -1 || //commented on Jan 11, 2018 for telenor client
                                newLocationArr[1].indexOf("FacebookReport") !== -1 ||
                                newLocationArr[1].indexOf("TwitterReport") !== -1 ||
                                newLocationArr[1].indexOf("EmailSettings") !== -1 ||
                                newLocationArr[1].indexOf("TagsSettings") !== -1 ||
                                newLocationArr[1].indexOf("PatternCustomization") !== -1
                            ) && !$rootScope.IsAdmin) ||
                              (newLocationArr[1].indexOf("ScheduledMessages") !== -1 && !$rootScope.PostSchedule) ||
                              (newLocationArr[1].indexOf("QueuedMessages") !== -1 && !$rootScope.PostQueue) ||
                              (newLocationArr[1].indexOf("Schedular") !== -1 && !($rootScope.PostQueue || $rootScope.PostSchedule))
                      ){
                            $location.path("/ErrorPage");
                      }
                  }
              }
          }
      });

      $rootScope.AssignRights = function (Rights, Reports) {
          $rootScope.PostRespond = false;
          $rootScope.PostEdit = false;
          $rootScope.PostSchedule = false;
          $rootScope.PostQueue = false; //publish without approval
          $rootScope.PostApproval = false; //group manager

          if (Rights != null) {
              for (i = 0; i < Rights.length; i++) {
                  if (Rights[i] == 1) {
                      $rootScope.PostRespond = true;
                  }
                  if (Rights[i] == 2) {
                      $rootScope.PostEdit = true;
                  }
                  if (Rights[i] == 3) {
                      $rootScope.PostSchedule = true;
                  }
                  if (Rights[i] == 4) {
                      $rootScope.PostQueue = true;
                  }
                  if (Rights[i] == 5) {
                      $rootScope.PostApproval = true;
                  }
              }
          }

          $rootScope.TwitTweets = false;
          $rootScope.TwitDMs = false;
          $rootScope.FBPosts = false; //manage message status
          $rootScope.FBComs = false; //publish without approval
          $rootScope.FBChats = false; //group manager
          $rootScope.IMPosts = false;
          $rootScope.IMComs = false;

          if (Reports != null) {
              for (i = 0; i < Reports.length; i++) {
                  if (Reports[i] == 1) {
                      $rootScope.TwitTweets = true;
                  }
                  if (Reports[i] == 2) {
                      $rootScope.TwitDMs = true;
                  }
                  if (Reports[i] == 3) {
                      $rootScope.FBPosts = true;
                  }
                  if (Reports[i] == 4) {
                      $rootScope.FBComs = true;
                  }
                  if (Reports[i] == 5) {
                      $rootScope.FBChats = true;
                  }
                  if (Reports[i] == 6) {
                      $rootScope.IMPosts = true;
                  }
                  if (Reports[i] == 7) {
                      $rootScope.IMComs = true;
                  }
              }
          }
      }

      // error = SPE
      if ($location.search().error != undefined) {
          var error = $location.search().error;
          if (error == 'SPE')
              var items = { ErrorTitle: "Error Adding Profile", ErrorMessage: "The profile you are adding is already linked with some other user." };
          else if (error == 'TRP') {
              var profile = $location.search().profile;
              var packageName = $location.search().package;
              var profileLimit = $location.search().limit;
              var profileType = $location.search().type;
              var items = { ErrorTitle: "Error Adding Profile", ErrorMessage: packageName + " account allows only " + profileLimit + " " + profileType + " Profile for each platform. You have already connected a profile " + profile };
          }

          var modalInstance = $uibModal.open({
              animation: true,
              templateUrl: '/Scripts/Views/Shared/_ErrorPopUp.html',
              controller: '_ErrorPopUpController',

              resolve: {
                  items: function () {
                      return items;
                  }
              }
          });

          modalInstance.result.then(function (selectedItem) {
              //alert($window.location.hash.split('?')[0])
              $window.location.href = $window.location.hash.split('?')[0];
          }, function () {
              $log.info('Modal dismissed at: ' + new Date());
          });
      }
      if ($location.search().pagesAdd != undefined) {
          var items = $location.search().pagesAdd;
          var modalInstance = $uibModal.open({
              animation: true,
              templateUrl: '/Scripts/Views/Shared/_PagesAdd.html',
              controller: '_PagesAddController',

              resolve: {
                  items: function () {
                      return items;
                  }
              }
          });

          modalInstance.result.then(function (selectedItem) {
              var selected = selectedItem;
              alert(selected)
          }, function () {
              $log.info('Modal dismissed at: ' + new Date());
          });
      }
      if ($location.search().type != undefined) {
          if ($location.search().type == "personal") {
              var items = $location.search().pagesAdd;
              var modalInstance = $uibModal.open({
                  animation: true,
                  templateUrl: '/Scripts/Views/Shared/_PagesAdd.html',
                  controller: '_PagesAddController',

                  resolve: {
                      items: function () {
                          return items;
                      }
                  }
              });

              modalInstance.result.then(function (selectedItem) {
                  var selected = selectedItem;
                  alert(selected)
              }, function () {
                  $log.info('Modal dismissed at: ' + new Date());
              });
          }
      }
  }

]);

CRMApp.filter("trustUrl", ['$sce', function ($sce) {
    return function (recordingUrl) {
        return $sce.trustAsResourceUrl(recordingUrl);
    };
}]);

CRMApp.directive("noclick", function () {
    return {
        restrict: "A",
        scope: {
            updateFn: '&'
        },
        link: function (scope, element, attr) {
            scope.time = Date.now();

            element.on("mousedown mouseup", function (e) {
                if (e.type == "mousedown") {
                    scope.time = Date.now();
                }

                if (e.type == "mouseup") {
                    if (Date.now() - scope.time > 100) {
                        return false;
                    } else {
                        //console.log("clicked");
                        scope.updateFn();
                        scope.$apply();
                    }
                }
            });
        }
    }
});