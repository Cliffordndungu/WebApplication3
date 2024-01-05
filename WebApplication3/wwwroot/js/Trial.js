let stripe = Stripe("@StripeSettings.Value.PublishableKey");
/*var username = '@UserManager.GetUserName(User)';*/
var quantity = 5;
/*let trialenddate = document.querySelector('#trialendate');*/

$(document).ready(function () {
    $('#confirmButton').on('click', function (event) {
        event.preventDefault();
        console.log("clicked");
        var productId = document.querySelector('#productid').textContent;
        var priceId = document.querySelector('#priceid').textContent;
       
        if (productId !== '' && priceId !== '') {
            console.log("about to go for processing");
            CreateSubscriptionWithTrial(username, productId, quantity);
        }
    });
});

console.log("about to start subscriptions");

// BEGIN Subscription Functions
function CreateSubscriptionWithTrial(username, productId, quantity, tedate) {
   /* loadingScreen(true);*/
    console.log("creating subscription");
    $.ajax({
        type: "POST",
        url: "/CheckoutApi/TrialSubscriptionCreateHandler",
        cache: false,
        data: {
            "Username": username,
            "Productid": productId,
            "Quantity": quantity,
            "TrialEndDate": tedate

        },
        dataType: "json",
        success: function (response) {
            response = JSON.parse(response);
            console.log("trial order success");
            window.location.href = "/Order/CompleteOrder";
        },
        error: function (response) {
            var exception = JSON.parse(response.responseText);
            console.log("payment error");
            errorMessage(exception);
        },
    });
}

//var progress = document.getElementById('Progress');
//var progressBackground = document.getElementById('ProgressBackground');

//function loadingScreen(visible) {
//    if (visible) {
//        progress.style.display = 'block';
//        progressBackground.style.display = 'block';
//    } else {
//        progress.style.display = 'none';
//        progressBackground.style.display = 'none';
//    }
//}
