//1.
app.service('modservice', function ($http) {
    this.get = function () {
        
        var accesstoken = sessionStorage.getItem('accessToken');

        var authHeaders = {};
        if (accesstoken) {
            authHeaders.Authorization = 'Bearer ' + accesstoken;
        }

        var response = $http({
            url: "/api/ModelosAPI",
            method: "GET",
            headers: authHeaders
        });
        return response;
    };
});
