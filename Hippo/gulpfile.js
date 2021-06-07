/// <binding AfterBuild='default' Clean='clean' />
/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');
var del = require('del');

var nodeRoot = './node_modules/';
var targetPath = './wwwroot/lib/';

gulp.task('clean', function () {
    return del([targetPath + '/**/*']);
});

gulp.task('default', async function() {
    gulp.src(nodeRoot + "bootstrap/dist/js/*").pipe(gulp.dest(targetPath + "/bootstrap/dist/js"));
    gulp.src(nodeRoot + "bootstrap/dist/css/*").pipe(gulp.dest(targetPath + "/bootstrap/dist/css"));

    gulp.src(nodeRoot + "@fortawesome/fontawesome-free/css/*").pipe(gulp.dest(targetPath + "/fontawesome/dist/css"));
    gulp.src(nodeRoot + "@fortawesome/fontawesome-free/js/*").pipe(gulp.dest(targetPath + "/fontawesome/dist/js"));
    gulp.src(nodeRoot + "@fortawesome/fontawesome-free/webfonts/*").pipe(gulp.dest(targetPath + "/fontawesome/dist/webfonts"));

    gulp.src(nodeRoot + "jquery/dist/jquery.js").pipe(gulp.dest(targetPath + "/jquery/dist"));
    gulp.src(nodeRoot + "jquery/dist/jquery.min.js").pipe(gulp.dest(targetPath + "/jquery/dist"));
    gulp.src(nodeRoot + "jquery/dist/jquery.min.map").pipe(gulp.dest(targetPath + "/jquery/dist"));

    gulp.src(nodeRoot + "jquery-validation/dist/*.js").pipe(gulp.dest(targetPath + "/jquery-validation/dist"));

    gulp.src(nodeRoot + "jquery-validation-unobtrusive/dist/*.js").pipe(gulp.dest(targetPath + "/jquery-validation-unobtrusive"));

    gulp.src(nodeRoot + "xterm/css/xterm.css").pipe(gulp.dest(targetPath + "/xterm/dist/css"));
    gulp.src(nodeRoot + "xterm/lib/xterm.js").pipe(gulp.dest(targetPath + "/xterm/dist/js"));
});
