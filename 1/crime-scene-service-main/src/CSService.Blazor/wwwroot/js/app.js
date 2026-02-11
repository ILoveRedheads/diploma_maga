async function downloadFile(stream) {
    const arrayBuffer = await stream.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = 'reports.zip';
    link.click();
    link.remove();
}