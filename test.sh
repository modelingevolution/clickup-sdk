#!/bin/bash

# ClickUp SDK Test Runner Script
# This script runs both unit tests and integration tests

echo "========================================="
echo "ClickUp SDK Test Runner"
echo "========================================="

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check if we're in the right directory
if [ ! -f "ModelingEvolution.ClickUp.sln" ]; then
    echo -e "${RED}Error: ModelingEvolution.ClickUp.sln not found!${NC}"
    echo "Please run this script from the ClickUp SDK root directory."
    exit 1
fi

# Function to run tests
run_tests() {
    local test_type=$1
    local filter=$2
    
    echo -e "\n${YELLOW}Running $test_type...${NC}"
    
    if dotnet test --filter "$filter" --logger "console;verbosity=normal"; then
        echo -e "${GREEN}✓ $test_type passed!${NC}"
        return 0
    else
        echo -e "${RED}✗ $test_type failed!${NC}"
        return 1
    fi
}

# Build the solution first
echo -e "\n${YELLOW}Building solution...${NC}"
if dotnet build --configuration Release; then
    echo -e "${GREEN}✓ Build successful!${NC}"
else
    echo -e "${RED}✗ Build failed!${NC}"
    exit 1
fi

# Run unit tests
UNIT_TEST_RESULT=0
if ! run_tests "Unit Tests" "FullyQualifiedName!~IntegrationTests"; then
    UNIT_TEST_RESULT=1
fi

# Check if integration tests should be run
if [ "$1" == "--integration" ] || [ "$1" == "-i" ]; then
    # First, temporarily remove Skip attributes
    echo -e "\n${YELLOW}Enabling integration tests...${NC}"
    
    # Create backup of the test file
    cp tests/ModelingEvolution.ClickUp.Tests/IntegrationTests/ClickUpClientIntegrationTests.cs \
       tests/ModelingEvolution.ClickUp.Tests/IntegrationTests/ClickUpClientIntegrationTests.cs.bak
    
    # Remove Skip attributes
    sed -i 's/\[Fact(Skip = "Integration test - requires API token")\]/[Fact]/g' \
        tests/ModelingEvolution.ClickUp.Tests/IntegrationTests/ClickUpClientIntegrationTests.cs
    
    # Run integration tests
    INTEGRATION_TEST_RESULT=0
    if ! run_tests "Integration Tests" "FullyQualifiedName~IntegrationTests"; then
        INTEGRATION_TEST_RESULT=1
    fi
    
    # Restore the original file
    mv tests/ModelingEvolution.ClickUp.Tests/IntegrationTests/ClickUpClientIntegrationTests.cs.bak \
       tests/ModelingEvolution.ClickUp.Tests/IntegrationTests/ClickUpClientIntegrationTests.cs
    
    echo -e "\n${YELLOW}Integration tests have been re-disabled.${NC}"
else
    echo -e "\n${YELLOW}Skipping integration tests. Use --integration or -i to run them.${NC}"
    INTEGRATION_TEST_RESULT=0
fi

# Summary
echo -e "\n========================================="
echo "Test Summary:"
echo "========================================="

if [ $UNIT_TEST_RESULT -eq 0 ]; then
    echo -e "${GREEN}✓ Unit Tests: PASSED${NC}"
else
    echo -e "${RED}✗ Unit Tests: FAILED${NC}"
fi

if [ "$1" == "--integration" ] || [ "$1" == "-i" ]; then
    if [ $INTEGRATION_TEST_RESULT -eq 0 ]; then
        echo -e "${GREEN}✓ Integration Tests: PASSED${NC}"
    else
        echo -e "${RED}✗ Integration Tests: FAILED${NC}"
    fi
fi

echo "========================================="

# Exit with error if any tests failed
if [ $UNIT_TEST_RESULT -ne 0 ] || [ $INTEGRATION_TEST_RESULT -ne 0 ]; then
    exit 1
fi

exit 0