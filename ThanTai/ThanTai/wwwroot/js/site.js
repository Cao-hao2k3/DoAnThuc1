document.querySelectorAll(".category-item").forEach(item => {
    item.addEventListener("mouseover", function () {
        document.querySelectorAll(".category-item").forEach(i => i.classList.remove("active"));
        this.classList.add("active");

        let category = this.getAttribute("data-category");
        document.querySelectorAll(".category-content").forEach(c => c.classList.remove("active"));
        document.querySelector("." + category).classList.add("active");
    });
});
