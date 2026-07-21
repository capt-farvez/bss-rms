// Client-side image downscaling/re-encoding. Images are stored as base64 inside
// database rows, so every megabyte uploaded is paid for on each query that touches
// the row — shrink photos before they ever leave the browser.
export function compressImage(file: File, maxDimension = 800, quality = 0.75): Promise<string> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onerror = () => reject(new Error('Could not read image file'));
    reader.onload = () => {
      const dataUrl = reader.result as string;
      const img = new Image();
      // Not decodable as an image in this browser — fall back to the raw file
      img.onerror = () => resolve(dataUrl);
      img.onload = () => {
        let width = img.naturalWidth;
        let height = img.naturalHeight;
        if (width > maxDimension || height > maxDimension) {
          const scale = Math.min(maxDimension / width, maxDimension / height);
          width = Math.round(width * scale);
          height = Math.round(height * scale);
        }

        const canvas = document.createElement('canvas');
        canvas.width = width;
        canvas.height = height;
        const ctx = canvas.getContext('2d');
        if (!ctx) {
          resolve(dataUrl);
          return;
        }

        // JPEG has no alpha channel — flatten transparent PNG/WebP onto white
        ctx.fillStyle = '#ffffff';
        ctx.fillRect(0, 0, width, height);
        ctx.drawImage(img, 0, 0, width, height);

        const compressed = canvas.toDataURL('image/jpeg', quality);
        // Tiny originals can grow when re-encoded — keep whichever is smaller
        resolve(compressed.length < dataUrl.length ? compressed : dataUrl);
      };
      img.src = dataUrl;
    };
    reader.readAsDataURL(file);
  });
}
