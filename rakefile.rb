ripple_root = File.dirname(__FILE__)
package_root = File.join(ripple_root, "packages")
code_root = File.expand_path(File.join(ripple_root, "../"))

task :ripple => [:clean] do
  puts "rippling #{ripple_root} to #{code_root} from #{package_root}"

  libs = ['fubucore', 'bottles', 'fubumvc', 'fubuvalidation', 'fubufastpack']

  libs.each do |lib|
    lib_dir = File.join code_root, lib
    raise "Could not find dependent library: #{lib_dir}" unless File.directory?(lib_dir)
  end

  libs.each do |lib|
    run_in File.join(code_root, lib), "rake nuget:pull nuget:ripple \"NUGET_HUB=#{package_root}\""
  end

  run_in File.join(code_root, "blue"), "rake nuget:pull \"NUGET_HUB=#{package_root}\""
end

task :clean do
  rm_rf Dir["#{package_root}/*"], :verbose => false
end

def run_in(working_dir, cmd)
  sh "run_in_path.cmd #{working_dir.gsub('/','\\')} #{cmd}"
end
