document.getElementById("processBtn").addEventListener("click", async () => {
    const fileInput = document.getElementById("csvFile");
    const file = fileInput.files[0];

    if (!file) {
        alert("Please select a CSV file first.");
        return;
    }

    const reader = new FileReader();
    reader.onload = async function (e) {
        const text = e.target.result;
        const rows = text.split("\n").map(r => r.trim()).filter(r => r.length > 0);

        const headers = rows[0].split(",");
        const dataRows = rows.slice(1);

        let allResults = [];
        let allExtraHeaders = new Set();

        // Show progress UI
        const progressContainer = document.getElementById("progressContainer");
        const progressFill = document.getElementById("progressFill");
        const statusText = document.getElementById("statusText");
        progressContainer.style.display = "block";
        statusText.textContent = "Processing...";
        progressFill.style.width = "0%";

        for (let i = 0; i < dataRows.length; i++) {
            const row = dataRows[i];
            const cols = row.split(",");
            const siteid = cols[headers.indexOf("siteid")];
            const latitude = cols[headers.indexOf("latitude")];
            const longitude = cols[headers.indexOf("longitude")];
            const term = cols[headers.indexOf("term")];

            try {
                const cacheBuster = new Date().getTime();
                const url = `http://tectonicsstaging.dedicated.co.za/LAAPI/Apps/getfeasibility?key=0e43d49c1c384ae2bb3b41711d7a0626&latitude_y=${latitude}&longitude_x=${longitude}&term=${term}&_=${cacheBuster}`;

                const response = await fetch(url, {
                    method: "GET",
                    cache: "no-store",
                    headers: {
                        "Cache-Control": "no-cache, no-store, must-revalidate",
                        "Pragma": "no-cache",
                        "Expires": "0"
                    }
                });

                if (!response.ok) {
                    console.error(`API error for siteid ${siteid}:`, response.status);
                    continue;
                }

                const jsonData = await response.json();

                for (const item of jsonData) {
                    Object.keys(item).forEach(k => allExtraHeaders.add(k));
                    let rowData = { siteid, latitude, longitude, bandwidth: cols[headers.indexOf("bandwidth")], term };
                    Object.assign(rowData, item);
                    allResults.push(rowData);
                }

            } catch (err) {
                console.error(`Error fetching for siteid ${siteid}:`, err);
            }

            // Update progress
            const percent = Math.round(((i + 1) / dataRows.length) * 100);
            progressFill.style.width = percent + "%";
            statusText.textContent = `Processing ${i + 1} of ${dataRows.length} (${percent}%)`;
        }

        // Final headers
        const finalHeaders = [...headers, ...Array.from(allExtraHeaders).filter(h => !headers.includes(h))];

        const finalCsv = [
            finalHeaders.join(","),
            ...allResults.map(r => finalHeaders.map(h => r[h] ?? "").join(","))
        ].join("\n");

        // Download
        downloadCsv(finalCsv, "modified.csv");

        // Done
        statusText.textContent = "✅ Done! File downloaded.";
    };

    reader.readAsText(file);
});

function downloadCsv(content, filename) {
    const blob = new Blob([content], { type: "text/csv" });
    const url = URL.createObjectURL(blob);

    const a = document.createElement("a");
    a.href = url;
    a.download = filename;
    a.click();

    URL.revokeObjectURL(url);
}
