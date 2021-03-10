#!/usr/bin/bash
cat /proc/cpuinfo | grep "model name" | sort -u
logicalCores=$(cat /proc/cpuinfo | grep "model name" | wc -l)
physicalCores=$(cat /proc/cpuinfo | grep "physical id" | sort -u | wc -l)

echo "Cores físicos: $physicalCores"
echo "Cores lógicos: $logicalCores"