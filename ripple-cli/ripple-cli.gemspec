# coding: utf-8
Gem::Specification.new do |spec|
  spec.name          = "ripple-cli"
  spec.version       = BUILD_NUMBER
  spec.authors       = ["Jeremy Miller", "Josh Arnold", "Ryan Rauh", "Matthew Smith"]
  spec.email         = ["fubumvc-devel@googlegroups.com"]
  spec.description   = "Ripple is a tool that wraps Nuget with workflow more conducive to upstream/downstream development across code repositories"
  spec.summary       = "Improved dependency management with Nuget"
  spec.homepage      = "http://fubu-project.org"
  spec.license       = "Apache 2.0"

  spec.files         = Dir.glob("bin/**/*").to_a
  spec.executables   = ["ripple"]
  spec.test_files    = spec.files.grep(%r{^(test|spec|features)/})
  spec.require_paths = ["lib"]

  spec.add_development_dependency "bundler", "~> 1.3.5"
  spec.add_development_dependency "rake"
end
