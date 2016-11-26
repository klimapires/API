 

//The Service Containing functions for Register User and 
//User Login
app.service('logoffservice', function ($http) {

});


app.controller('logoffcontroller', function ($scope, logoffservice) {

    $scope.Miapis = "aaaxx";

    $scope.userName = sessionStorage.getItem('userName');
    //$scope.Message = "";
    $scope.logoff = function () {
        sessionStorage.removeItem('accessToken');
        window.location.href = '/Account/LogOff';
    };

});
