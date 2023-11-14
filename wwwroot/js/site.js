<script>
    function validateForm(event) {
  
  var phone = document.getElementById("phone").value;
    var password = document.getElementById("password").value;
    var repassword = document.getElementById("Repassword").value;


    var phoneRegex = /^\d{10}$/;


    if (!phoneRegex.test(phone)) {
        event.preventDefault();
    document.getElementById("phone-error").innerText = "Please enter a valid phone number.";
  }

    if (password !== repassword) {
        event.preventDefault();
    document.getElementById("Repassword-error").innerText = "Passwords do not match.";
  }
}
</script>