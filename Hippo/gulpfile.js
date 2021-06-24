/// <binding AfterBuild='default' Clean='clean' />
/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');
var sass = require('gulp-dart-sass');
var del = require('del');

var nodeRoot = './node_modules/';
var targetPath = './wwwroot/lib/';

gulp.task('clean', function () {
    return del([targetPath + '/**/*']);
});

// compile the css
gulp.task('sass', function () {
return gulp.src('./assets/styles/hippo.scss')
    .pipe(sass().on('error', sass.logError))
    .pipe(gulp.dest('./wwwroot/styles/'));
});
gulp.task('watch', function() {
    gulp.watch('./assets/styles/hippo.scss', gulp.series('sass'));
});

// move assets from node_modules to wwwroot
gulp.task('copy', async function() {
    gulp.src(nodeRoot + "bulma/**/*").pipe(gulp.dest(targetPath + "/bulma/"));

    gulp.src(nodeRoot + "bootstrap/dist/js/*").pipe(gulp.dest(targetPath + "/bootstrap/dist/js"));
    gulp.src(nodeRoot + "bootstrap/dist/css/*").pipe(gulp.dest(targetPath + "/bootstrap/dist/css"));

    gulp.src(nodeRoot + "font-awesome/**/*").pipe(gulp.dest(targetPath + "/font-awesome/"));

    gulp.src(nodeRoot + "jquery/dist/jquery.js").pipe(gulp.dest(targetPath + "/jquery/dist"));
    gulp.src(nodeRoot + "jquery/dist/jquery.min.js").pipe(gulp.dest(targetPath + "/jquery/dist"));
    gulp.src(nodeRoot + "jquery/dist/jquery.min.map").pipe(gulp.dest(targetPath + "/jquery/dist"));

    gulp.src(nodeRoot + "jquery-validation/dist/*.js").pipe(gulp.dest(targetPath + "/jquery-validation/dist"));

    gulp.src(nodeRoot + "jquery-validation-unobtrusive/dist/*.js").pipe(gulp.dest(targetPath + "/jquery-validation-unobtrusive"));

    gulp.src(nodeRoot + "xterm/css/xterm.css").pipe(gulp.dest(targetPath + "/xterm/dist/css"));
    gulp.src(nodeRoot + "xterm/lib/*").pipe(gulp.dest(targetPath + "/xterm/dist/js"));
});

gulp.task('default', gulp.series('copy', 'sass'), function() { });
