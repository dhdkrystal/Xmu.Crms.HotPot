var gulp = require("gulp");
var dest_css = "./wwwroot/css/vendor/";
var dest_js = "./wwwroot/js/";
gulp.task("css", function () {
    gulp.src("./node_modules/tether/dist/css/tether.min.css")
        .pipe(gulp.dest(dest_css));
    gulp.src("./node_modules/bootstrap/dist/css/bootstrap.min.css")
        .pipe(gulp.dest(dest_css));
});

gulp.task("js", function () {
    gulp.src("./node_modules/jquery/dist/jquery.min.js")
        .pipe(gulp.dest(dest_js));
    gulp.src("./node_modules/tether/dist/js/tether.min.js")
        .pipe(gulp.dest(dest_js));
    gulp.src("./node_modules/bootstrap/dist/js/bootstrap.min.js")
        .pipe(gulp.dest(dest_js));
    gulp.src("./node_modules/bootstrap/dist/js/bootstrap.bundle.min.js")
        .pipe(gulp.dest(dest_js));
});

gulp.task("default", ["css", "js"]);