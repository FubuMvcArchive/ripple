require 'buildsupport/nuget.rb'
@ripple_root = File.dirname(__FILE__)
@package_root = File.join(@ripple_root, "packages")
@code_root = File.expand_path(File.join(@ripple_root, "../"))

namespace :ripple do
  desc "Build project, push, push new package to nuget.org, consume in next project. Repeat"
  task :start => [:reset] do
    # 1) Determine dependency graph
    libs = get_libs_to_ripple

    # 2) Validate that all dependencies are available locally
    validate_libs_exist libs

    # 3) Write list of jobs
    init_ripple_jobs libs

    # 4) Start the first job
    Rake::Task["ripple:nextjob"].execute
  end

  desc "Build each project and push build output to downstream projects"
  task :startlocal => [:reset] do
    libs = get_libs_to_ripple
    validate_libs_exist libs
    libs.each do |lib|
      run_in File.join(@code_root, lib), "rake ripple:local \"NUGET_HUB=#{@package_root}\""
    end
  end

  def get_libs_to_ripple
    ['fubucore', 'bottles', 'fubumvc', 'fubuvalidation', 'fubufastpack']
  end

  def validate_libs_exist(libs)
    libs.each do |lib|
      lib_dir = File.join @code_root, lib
      raise "Could not find dependent library: #{lib_dir}" unless File.directory?(lib_dir)
    end
  end

  task :nextjob do
    next_job = get_next_job 
    if next_job
      run_in File.join(@code_root, next_job), "rake ripple:public \"NUGET_HUB=#{@package_root}\""
    else
      puts "Ripple complete."
    end
  end

  task :reset do
    rm_rf Dir["#{@package_root}/*"], :verbose => false
  end
end

