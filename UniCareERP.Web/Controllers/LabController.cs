using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UniCareERP.Application.Services.Lab;
using UniCareERP.Application.DTOs.Lab;

namespace UniCareERP.Web.Controllers
{
    public class LabController : Controller
    {
        private readonly ILabTestService _labTestService;
        private readonly ITestRequestService _testRequestService;
        private readonly ITestResultService _testResultService;

        public LabController(
            ILabTestService labTestService,
            ITestRequestService testRequestService,
            ITestResultService testResultService)
        {
            _labTestService = labTestService;
            _testRequestService = testRequestService;
            _testResultService = testResultService;
        }

        // LabTest actions
        public async Task<IActionResult> Index()
        {
            var labTests = await _labTestService.GetAllLabTestsAsync();
            return View(labTests);
        }

        public async Task<IActionResult> LabTestDetails(Guid id)
        {
            var labTest = await _labTestService.GetLabTestByIdAsync(id);
            if (labTest == null) return NotFound();
            return View(labTest);
        }

        public IActionResult CreateLabTest()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLabTest(LabTestDto labTestDto)
        {
            if (ModelState.IsValid)
            {
                await _labTestService.CreateLabTestAsync(labTestDto);
                return RedirectToAction(nameof(Index));
            }
            return View(labTestDto);
        }

        public async Task<IActionResult> EditLabTest(Guid id)
        {
            var labTest = await _labTestService.GetLabTestByIdAsync(id);
            if (labTest == null) return NotFound();
            return View(labTest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLabTest(Guid id, LabTestDto labTestDto)
        {
            if (id != labTestDto.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                await _labTestService.UpdateLabTestAsync(labTestDto);
                return RedirectToAction(nameof(Index));
            }
            return View(labTestDto);
        }

        public async Task<IActionResult> DeleteLabTest(Guid id)
        {
            var labTest = await _labTestService.GetLabTestByIdAsync(id);
            if (labTest == null) return NotFound();
            return View(labTest);
        }

        [HttpPost, ActionName("DeleteLabTest")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLabTestConfirmed(Guid id)
        {
            await _labTestService.DeleteLabTestAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // TestRequest actions
        public async Task<IActionResult> TestRequests()
        {
            var testRequests = await _testRequestService.GetAllTestRequestsAsync();
            return View(testRequests);
        }

        // Other actions for TestRequest and TestResult will be added here
    }
}
