CRMApp.controller('ClosedTaskController', ['$scope', '$window', '$location', 'TaskService', 'GlobalService', 'notify', function ($scope, $window, $location, TaskService, GlobalService, notify) {
    $scope.init = function () {
    };
    $scope.init();

    $scope.Search = function ()
    {
       // alert('Search Result.')
    }

    $scope.closedTasks =
        [
            {
                profileUrl: '/Content/images/in_ico16.png',
                logoUrl: '/Content/img/user2.jpg',
                userName: 'TALHA',
                profileName: 'chocOholic',
                msg: 'Remember we met at CP, you were stuck in a confidential task and I was playing batman there :p',
                assignedTo: 'Amber Ashraf',
                priority: 'High',
                classification: 'Sales Lead',
                lastUpdated: 'Mar 08, 2016 , 12:35PM'
            },
             {
                 profileUrl: '/Content/images/in_ico16.png',
                 logoUrl: '/Content/img/user2.jpg',
                 userName: 'TALHA',
                 profileName: 'chocOholic',
                 msg: 'Remember we met at CP, you were stuck in a confidential task and I was playing batman there :p',
                 assignedTo: 'Amber Ashraf',
                 priority: 'High',
                 classification: 'Sales Lead',
                 lastUpdated: 'Mar 08, 2016 , 12:35PM'
             },
              {
                  profileUrl: '/Content/images/in_ico16.png',
                  logoUrl: '/Content/img/user2.jpg',
                  userName: 'TALHA',
                  profileName: 'chocOholic',
                  msg: 'Remember we met at CP, you were stuck in a confidential task and I was playing batman there :p',
                  assignedTo: 'Amber Ashraf',
                  priority: 'Medium',
                  classification: 'Sales Lead',
                  lastUpdated: 'Mar 08, 2016 , 12:35PM'
              },
               {
                   profileUrl: '/Content/images/in_ico16.png',
                   logoUrl: '/Content/img/user2.jpg',
                   userName: 'TALHA',
                   profileName: 'chocOholic',
                   msg: 'Remember we met at CP, you were stuck in a confidential task and I was playing batman there :p',
                   assignedTo: 'Amber Ashraf',
                   priority: 'Low',
                   classification: 'Sales Lead',
                   lastUpdated: 'Mar 08, 2016 , 12:35PM'
               }
        ];

    $scope.taskAssignee = [
   {
       id: 1,
       value: 'Member A'
   },
   {
       id: 2,
       value: 'Member B'
   },
    {
        id: 3,
        value: 'Member C'
    },
   {
       id: 4,
       value: 'Member D'
   }
    ];
    $scope.taskPriority = [
     {
         id: 1,
         value: '! High'
     },
   {
       id: 2,
       value: 'Medium'
   },
   {
       id: 3,
       value: 'Low'
   }];
    $scope.taskClassification = [
    {
        id: 1,
        value: 'Sales Lead'
    },
   {
       id: 2,
       value: 'After Sales'
   },
     {
         id: 3,
         value: 'Compliment'
     },
   {
       id: 4,
       value: 'Complaint'
   },
     {
         id: 5,
         value: 'General Inquiry'
     },
   {
       id: 6,
       value: 'Referral'
   },
     {
         id: 7,
         value: 'Review'
     }];

    $scope.attachedProfiles = [
    {
        id: 1,
        value: 'Adeel'
    },
  {
      id: 2,
      value: 'Gill'
  },
  {
      id: 3,
      value: 'Ayesha'
  }];
}]);