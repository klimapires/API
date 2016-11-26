//1.
app.service('usersaspservice', function ($http) {
    this.get = function () {
        
        //var accesstoken = sessionStorage.getItem('accessToken');

        var authHeaders = {};
        //if (accesstoken) {
        //    authHeaders.Authorization = 'Bearer ' + accesstoken;
        //}

        var response = $http({
            url: "/api/AccountAPI/ListaUsers",
            method: "GET",
            headers: authHeaders
        });
        return response;
    };
});
