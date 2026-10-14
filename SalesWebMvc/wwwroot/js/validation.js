// Adaptadores client-side para os atributos de validação customizados
// (SalesWebMvc.Models.Validation). Espelham exatamente as regras do servidor.
(function ($) {
    'use strict';

    function parseIsoDate(value) {
        // <input type="date"> envia yyyy-MM-dd; criamos a data no fuso local
        const m = /^(\d{4})-(\d{2})-(\d{2})$/.exec(value);
        return m ? new Date(+m[1], +m[2] - 1, +m[3]) : null;
    }

    function today() {
        const d = new Date();
        return new Date(d.getFullYear(), d.getMonth(), d.getDate());
    }

    // [MinimumAge(min, max)]
    $.validator.addMethod('minage', function (value, element, params) {
        if (this.optional(element)) return true;
        const birth = parseIsoDate(value);
        if (!birth) return false;

        const now = today();
        let age = now.getFullYear() - birth.getFullYear();
        const hadBirthday = now.getMonth() > birth.getMonth()
            || (now.getMonth() === birth.getMonth() && now.getDate() >= birth.getDate());
        if (!hadBirthday) age--;

        return age >= params.min && age <= params.max;
    });

    $.validator.unobtrusive.adapters.add('minage', ['min', 'max'], function (options) {
        options.rules.minage = { min: parseInt(options.params.min, 10), max: parseInt(options.params.max, 10) };
        options.messages.minage = options.message;
    });

    // [NotInFuture]
    $.validator.addMethod('notfuture', function (value, element) {
        if (this.optional(element)) return true;
        const date = parseIsoDate(value);
        return !!date && date <= today();
    });

    $.validator.unobtrusive.adapters.addBool('notfuture');

})(jQuery);
