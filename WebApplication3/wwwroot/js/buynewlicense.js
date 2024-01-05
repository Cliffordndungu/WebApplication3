
    let buybox = document.querySelector('#buybox');
let buyLicensesForm = document.querySelector('#buyLicensesForm');
    let buyboxbody = document.querySelector('.buyboxbody');

buybox.addEventListener('click', () => {

        buyboxbody.classList.add('active');
    });


    buyLicensesForm.submit(function (e) {
        e.preventDefault();

        // Retrieve form data and handle form submission as needed
        let newQuantity = document.querySelector('#newQuantity').val();

        UpdateSubscription(subitemid, newQuantity);
    });

    /* Uncomment if closebox is used
    closebox.on('click', function () {
        buybox.removeClass('active');
    });
    */
    console.log("buylicense");
    function UpdateSubscription(subitemid, quantity) {
        $.ajax({
            type: "POST",
            url: "/Sub/UpdateSubscriptionHandler",
            cache: false,
            data: {
                "Subid": subitemid,
                "Quantity": quantity,
            },
            dataType: "json",
            success: function (response) {
               
                window.location.href = "/Order/CompleteOrder";
            },
            error: function (response) {
                console.log("Payment error");
                errorMessage(JSON.parse(response.responseText));
            },
        });
    }

    function errorMessage(exception) {
        // Handle error messages appropriately
        // You can display error messages to the user or handle them in a way that suits your application
        console.error("Error occurred: ", exception);
    }
});
