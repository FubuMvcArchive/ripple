require 'buildsupport/nuget.rb'
ripple_root = File.dirname(__FILE__)
package_root = File.join(ripple_root, "packages")
code_root = File.expand_path(File.join(ripple_root, "../"))

namespace :ripple do
  task :start => [:reset] do
    puts "rippling #{ripple_root} to #{code_root} from #{package_root}"

    # 1) Determine dependency graph
    libs = ['fubucore', 'bottles', 'fubumvc', 'fubuvalidation', 'fubufastpack']

    # 2) Validate that all dependencies are available locally
    libs.each do |lib|
      lib_dir = File.join code_root, lib
      raise "Could not find dependent library: #{lib_dir}" unless File.directory?(lib_dir)
    end

    # 3) Write list of jobs
    init_ripple_jobs libs

    # 4) Start the first job
    Rake::Task["ripple:nextjob"].execute

  #  libs.each do |lib|
  #    run_in File.join(code_root, lib), "rake nuget:pull nuget:ripple \"NUGET_HUB=#{package_root}\""
  #  end

  #  run_in File.join(code_root, "blue"), "rake nuget:pull \"NUGET_HUB=#{package_root}\""
  end

  task :nextjob do
    next_job = get_next_job 
    if next_job
      run_in File.join(code_root, next_job), "rake ripple:public \"NUGET_HUB=#{package_root}\""
    else
      puts "Ripple complete."
    end
  end

  task :reset do
    rm_rf Dir["#{package_root}/*"], :verbose => false
  end
end

