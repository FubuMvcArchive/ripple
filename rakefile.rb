ripple_root = File.dirname(__FILE__)
package_root = File.join(ripple_root, "packages")
code_root = File.expand_path(File.join(ripple_root, "../"))

task :ripple do
  puts "rippling #{ripple_root} to #{code_root} from #{package_root}"

  libs = ['fubucore', 'bottles', 'fubumvc', 'fubuvalidation', 'fubufastpack']
  libs.each do |lib|
    run_in File.join(code_root, lib), "rake nuget:ripple \"NUGET_HUB=#{package_root}\""
  end
end

def run_in(working_dir, cmd)
  raise "Could not find #{working_dir} - aborting" unless File.directory? working_dir
  sh "run_in_path.cmd #{working_dir.gsub('/','\\')} #{cmd}"
end
