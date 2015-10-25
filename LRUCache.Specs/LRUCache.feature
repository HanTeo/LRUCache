
#Key features
#   The cache should implement the same IProvider interface so we can easily integrate it into existing code
#
#    It should populate on demand
#    It should keep no more than 10 entries in memory at any time. When the 11th distinct key is requested, the least recently queried key/value pair should be evicted from the cache
#
#Please bear in mind the following:
#
#    The cache should be safe when called by multiple threads
#    If two or more threads request the same key, only one call to the underlying IProvider implementation should be made
#    Ideally, the locking should be fine-grained, so if two threads request different keys, the retrieval should operate in parallel
#    The provider is not 100% reliable, it can fail on occasion with an exception

Feature: LRUCache
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

@mytag
Scenario: Add two numbers
	Given I have entered 50 into the calculator
	And I have entered 70 into the calculator
	When I press add
	Then the result should be 120 on the screen
