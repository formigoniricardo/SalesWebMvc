// Comportamentos globais do layout: sidebar mobile, toasts (SweetAlert2), confirmação de exclusão, atalho de busca.
(function () {
    'use strict';

    // ---------- Sidebar off-canvas (mobile) ----------
    const sidebar = document.getElementById('sidebar');
    const backdrop = document.getElementById('sidebar-backdrop');
    const toggle = document.getElementById('sidebar-toggle');

    function setSidebar(open) {
        sidebar.classList.toggle('open', open);
        backdrop.classList.toggle('show', open);
        toggle?.setAttribute('aria-expanded', String(open));
    }

    toggle?.addEventListener('click', () => setSidebar(!sidebar.classList.contains('open')));
    backdrop?.addEventListener('click', () => setSidebar(false));
    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') setSidebar(false);

        // "/" foca a busca global (exceto quando já está digitando em um campo)
        const tag = document.activeElement?.tagName;
        if (e.key === '/' && tag !== 'INPUT' && tag !== 'TEXTAREA' && tag !== 'SELECT') {
            e.preventDefault();
            document.querySelector('.topbar-search input')?.focus();
        }
    });

    // ---------- Toasts vindos do servidor (TempData) ----------
    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3500,
        timerProgressBar: true,
        didOpen: (el) => {
            el.addEventListener('mouseenter', Swal.stopTimer);
            el.addEventListener('mouseleave', Swal.resumeTimer);
        }
    });
    window.appToast = (type, message) => Toast.fire({ icon: type, title: message });

    try {
        const data = JSON.parse(document.getElementById('toast-data')?.textContent || '{}');
        if (data.message) window.appToast(data.type || 'info', data.message);
    } catch { /* sem toast */ }

    // ---------- Exclusão com confirmação ----------
    // Uso: <button data-delete-url="/Sellers/Delete/5" data-delete-name="Bob Brown">
    document.addEventListener('click', async (e) => {
        const btn = e.target.closest('[data-delete-url]');
        if (!btn) return;
        e.preventDefault();

        const name = btn.dataset.deleteName || 'este registro';
        const result = await Swal.fire({
            title: 'Confirmar exclusão',
            html: `Tem certeza que deseja excluir <strong></strong>?<br><small>Esta ação não pode ser desfeita.</small>`,
            icon: 'warning',
            iconColor: '#f87171',
            showCancelButton: true,
            confirmButtonText: '<i class="bi bi-trash3 me-1"></i> Excluir',
            cancelButtonText: 'Cancelar',
            reverseButtons: true,
            focusCancel: true,
            buttonsStyling: false,
            customClass: { confirmButton: 'btn btn-danger ms-2', cancelButton: 'btn btn-ghost' },
            didOpen: (popup) => { popup.querySelector('strong').textContent = name; } // textContent evita XSS
        });

        if (result.isConfirmed) {
            const form = document.getElementById('delete-form');
            form.action = btn.dataset.deleteUrl;
            form.submit();
        }
    });

    // ---------- Selects de filtro que submetem sozinhos ----------
    document.querySelectorAll('[data-autosubmit]').forEach((el) =>
        el.addEventListener('change', () => el.form.requestSubmit()));
})();
