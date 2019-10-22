cd ../bin/Release/netcoreapp3.0
for %%f in (../../../stats_analyse/*.json) do (
    ADS_Simulation -c "../../../stats_analyse/%%f"
)
pause