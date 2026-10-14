// Gráficos do Dashboard (Chart.js). Os dados chegam serializados pelo servidor em #dashboard-data.
(function () {
    'use strict';

    const data = JSON.parse(document.getElementById('dashboard-data').textContent);
    const css = getComputedStyle(document.documentElement);
    const ink = {
        text: css.getPropertyValue('--app-text').trim(),
        text2: css.getPropertyValue('--app-text-2').trim(),
        muted: css.getPropertyValue('--app-muted').trim(),
        grid: 'rgba(148, 163, 184, 0.08)',
        surface: css.getPropertyValue('--app-surface').trim()
    };

    const money = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' });
    const compact = new Intl.NumberFormat('pt-BR', { notation: 'compact', style: 'currency', currency: 'BRL', maximumFractionDigits: 1 });

    Chart.defaults.font.family = css.getPropertyValue('--bs-body-font-family');
    Chart.defaults.color = ink.muted;

    const tooltip = {
        backgroundColor: '#0b1120',
        borderColor: 'rgba(148, 163, 184, 0.22)',
        borderWidth: 1,
        titleColor: ink.text,
        bodyColor: ink.text2,
        padding: 12,
        cornerRadius: 10,
        displayColors: true,
        boxPadding: 4
    };

    // ---------- Linha/área: faturamento mensal (série única → sem legenda; o título nomeia) ----------
    const monthlyEl = document.getElementById('monthlyChart');
    if (monthlyEl) {
        const ctx = monthlyEl.getContext('2d');
        const gradient = ctx.createLinearGradient(0, 0, 0, monthlyEl.parentElement.clientHeight);
        gradient.addColorStop(0, 'rgba(57, 135, 229, 0.30)');
        gradient.addColorStop(1, 'rgba(57, 135, 229, 0)');

        new Chart(ctx, {
            type: 'line',
            data: {
                labels: data.monthly.labels,
                datasets: [{
                    label: 'Faturado',
                    data: data.monthly.values,
                    borderColor: '#3987e5',
                    backgroundColor: gradient,
                    borderWidth: 2,
                    fill: true,
                    tension: 0.35,
                    pointRadius: 4,
                    pointHoverRadius: 6,
                    pointBackgroundColor: '#3987e5',
                    pointBorderColor: ink.surface,
                    pointBorderWidth: 2
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                interaction: { mode: 'index', intersect: false }, // crosshair: hover em qualquer ponto da coluna
                plugins: {
                    legend: { display: false },
                    tooltip: { ...tooltip, callbacks: { label: (c) => ` Faturado: ${money.format(c.parsed.y)}` } }
                },
                scales: {
                    x: { grid: { display: false }, border: { color: 'rgba(148, 163, 184, 0.18)' }, ticks: { color: ink.muted } },
                    y: {
                        beginAtZero: true,
                        grid: { color: ink.grid },
                        border: { display: false },
                        ticks: { color: ink.muted, callback: (v) => compact.format(v), maxTicksLimit: 6 }
                    }
                }
            }
        });
    }

    // ---------- Rosca: distribuição por departamento (legenda HTML ao lado, com valores e %) ----------
    const deptEl = document.getElementById('departmentChart');
    if (deptEl) {
        const total = data.departments.values.reduce((a, b) => a + b, 0);
        new Chart(deptEl, {
            type: 'doughnut',
            data: {
                labels: data.departments.labels,
                datasets: [{
                    data: data.departments.values,
                    backgroundColor: data.departments.colors,
                    borderColor: ink.surface, // anel de 2px da cor da superfície separa as fatias
                    borderWidth: 2,
                    hoverOffset: 6
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: '68%',
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        ...tooltip,
                        callbacks: {
                            label: (c) => ` ${c.label}: ${money.format(c.parsed)} (${(c.parsed / total * 100).toFixed(1).replace('.', ',')}%)`
                        }
                    }
                }
            }
        });
    }
})();
