
Feature: TfL Journey Planner navigation


  @regression
  Scenario: Plan a journey and validate walking and cycling times
    Given I open the TfL Journey Planner
    When  I select "Leicester Square Underground Station" as the starting point
    And   I select "Covent Garden Underground Station" as the destination
    And   I plan my journey
    Then  walking and cycling journey times should be available
   
 @regression
  Scenario: Update journey preferences to minimise walking
    Given I open the TfL Journey Planner
    When  I select "Leicester Square Underground Station" as the starting point
    And   I select "Covent Garden Underground Station" as the destination
    And   I plan my journey
    When  I edit journey preferences
    And   I select routes with least walking
    And   I update the journey
    Then  the updated journey time should be valid

  @regression1
  Scenario: Verify complete access information at Covent Garden Underground Station
    Given I open the TfL Journey Planner
    When  I select "Leicester Square Underground Station" as the starting point
    And   I select "Covent Garden Underground Station" as the destination
    And   I plan my journey
    When  I edit journey preferences
    And   I select routes with least walking
    And   I update the journey
    When  I view details of the first journey option
    Then  the complete access information should be displayed

  @regression
   Scenario Outline: No journey results for invalid locations
    Given I open the TfL Journey Planner
    When I enter "<From>" as the starting point
    And  I enter "<To>" as the destination
    And  I plan my journey
    Then no journey results should be displayed

     Examples:
       | From                  | To              |
       | InvalidStation123     | Stratford       |
       | Leicester Square      | InvalidPlace999 |
       | InvalidStation123     | InvalidPlace999 |

  @regression
   Scenario: Journey cannot be planned without locations
    Given I open the TfL Journey Planner
    When  I leave the starting point and destination empty
    And   I plan my journey
    Then  the journey should not be planned