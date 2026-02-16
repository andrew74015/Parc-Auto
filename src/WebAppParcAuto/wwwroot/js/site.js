'use strict'

var App = {
   
    SetupUnobtrusiveValidation: function () {
        // https://kontext.tech/article/809/jquery-unobtrusive-validation-with-bootstrap-5-css
        // https://gist.github.com/carlin-q-scott/467444ee864f83a34af3ddc87086b89d (Used)
        $.validator.setDefaults({
            highlight: function (element, errorClass, validClass) {
                //console.log("highlight", element);
                // Only validation controls
                if (!$(element).hasClass('novalidation')) {
                    $(element).closest('.form-control').removeClass('is-valid').addClass('is-invalid');
                }
            },
            unhighlight: function (element, errorClass, validClass) {
                //console.log("unhighlight", element);
                // Only validation controls
                if (!$(element).hasClass('novalidation')) {
                    $(element).closest('.form-control').removeClass('is-invalid').addClass('is-valid');
                }
            }            
        });
    },


    SetupFormsSubmit: function () {
        // Fetch all the forms we want to apply custom Bootstrap validation styles to
        var forms = document.querySelectorAll('.needs-validation')

        // Loop over them and prevent submission
        Array.prototype.slice.call(forms)
            .forEach(function (form) {
                form.addEventListener('submit', function (event) {
                    if (!form.checkValidity()) {
                        event.preventDefault()
                        event.stopPropagation()
                    }

                    form.classList.add('was-validated')
                }, false)
            });
    },


    Select2: {
        PageSize: 50,

        AjaxRequestDelay: 750,

        SetDefaults: function () {
            $.fn.select2.defaults.set("debug", "false");
            $.fn.select2.defaults.set("cache", false);
            $.fn.select2.defaults.set("theme", "bootstrap-5");
            $.fn.select2.defaults.set("placeholder", "Selectați");
            $.fn.select2.defaults.set("minimumResultsForSearch", 7);
        },


        CustomAjaxData: function (params) {
            params.page = params.page || 1;
            return {
                searchTerm: params.term,
                pageSize: App.Select2.PageSize,
                pageNumber: params.page
            };
        },


        CustomAjaxProcessResults: function (data, params) {
            params.page = params.page || 1;
            return {
                results: data.Results,
                pagination: {
                    more: (params.page * App.Select2.PageSize) < data.Pagination.TotalRecords
                }
            };
        }
    },


    Go: function () {
        // Lansarea unei subaplicatii
        $(".subapp-item").on("click", function (e) {
            window.location.href = $(this).attr("link");
        });

        this.SetupUnobtrusiveValidation();

        this.SetupFormsSubmit();

        this.Select2.SetDefaults();
    }
};


$(function () {
    App.Go();
});
