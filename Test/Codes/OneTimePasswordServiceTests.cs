using FoodplannerModels.Account;
using FoodplannerModels.Codes;
using FoodplannerServices.Codes;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Codes
{
    public class OneTimePasswordServiceTests
    {
        private readonly Mock<IOneTimePasswordRepository> _mockOtpRepository;
        private readonly Mock<IChildrenRepository> _mockChildrenRepository;
        private readonly OneTimePasswordService _otpService;

        public OneTimePasswordServiceTests()
        {
            _mockOtpRepository = new Mock<IOneTimePasswordRepository>();
            _mockChildrenRepository = new Mock<IChildrenRepository>();

            _otpService = new OneTimePasswordService(
                _mockOtpRepository.Object,
                _mockChildrenRepository.Object
            );
        }
        [Fact]
        public async Task CheckIfCodeAlreadyExists_ReturnsExpectedValue()
        {
            // Arrange
            _mockOtpRepository
                .Setup(r => r.CheckIfCodeExistsAsync("123456"))
                .ReturnsAsync(true);

            // Act
            var result = await _otpService.CheckIfCodeAlreadyExists("123456");

            // Assert
            Assert.True(result);
        }
        [Fact]
        public async Task CreateOneTimePassword_InsertsOtpAndReturnsId()
        {
            // Arrange
            int expectedId = 42;

            _mockOtpRepository
                .Setup(r => r.CheckIfCodeExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false); // First generated code is unique

            _mockOtpRepository
                .Setup(r => r.InsertAsync(It.IsAny<OneTimePassword>()))
                .ReturnsAsync(expectedId);

            // Act
            var id = await _otpService.CreateOneTimePassword(10, null);

            // Assert
            Assert.Equal(expectedId, id);
            _mockOtpRepository.Verify(r => r.InsertAsync(It.IsAny<OneTimePassword>()), Times.Once);
        }

        [Fact]
        public async Task CreateOneTimePassword_LoopsUntilUniqueCodeFound()
        {
            // Arrange
            var callCount = 0;

            _mockOtpRepository
                .Setup(r => r.CheckIfCodeExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(() =>
                {
                    callCount++;
                    return callCount == 1; // first time returns true → simulate collision
                });

            _mockOtpRepository
                .Setup(r => r.InsertAsync(It.IsAny<OneTimePassword>()))
                .ReturnsAsync(1);

            // Act
            await _otpService.CreateOneTimePassword(10, null);

            // Assert
            // Must check at least twice (first collision, then unique)
            Assert.True(callCount >= 2);
        }
        [Fact]
        public async Task GetOneTimePassword_ReturnsOtp()
        {
            // Arrange
            var otp = new OneTimePassword { Code = "123456" };

            _mockOtpRepository
                .Setup(r => r.GetFromCodeAsync("123456"))
                .ReturnsAsync(otp);

            // Act
            var result = await _otpService.GetOneTimePassword("123456");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("123456", result.Code);
        }
        [Fact]
        public async Task RedeemOneTimePassword_WhenExpired_Returns0()
        {
            // Arrange
            _mockOtpRepository
                .Setup(r => r.CheckIfCodeExpiredAsync("111111"))
                .ReturnsAsync(true);

            // Act
            var result = await _otpService.RedeemOneTimePassword("111111");

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task RedeemOneTimePassword_ValidCode_AddsParentAndDeletesOtp()
        {
            // Arrange
            var otp = new OneTimePassword
            {
                Code = "111111",
                GeneratedBy = 10,
                UsedByUser = 20,
                Used = false
            };

            _mockOtpRepository
                .Setup(r => r.CheckIfCodeExpiredAsync("111111"))
                .ReturnsAsync(false);

            _mockOtpRepository
                .Setup(r => r.GetFromCodeAsync("111111"))
                .ReturnsAsync(otp);

            _mockChildrenRepository
                .Setup(r => r.AddParentToChildAsync(10, 20))
                .ReturnsAsync(99);

            // Act
            var result = await _otpService.RedeemOneTimePassword("111111");

            // Assert
            Assert.Equal(99, result);
            _mockOtpRepository.Verify(r => r.DeleteAsync("111111"), Times.Once);
            _mockChildrenRepository.Verify(r => r.AddParentToChildAsync(10, 20), Times.Once);
        }
        [Fact]
        public async Task UpdateOneTimePassword_WhenCodeExists_UpdatesOtp()
        {
            // Arrange
            var otp = new OneTimePassword { Code = "123456" };

            _mockOtpRepository
                .Setup(r => r.CheckIfCodeExistsAsync("123456"))
                .ReturnsAsync(true);

            _mockOtpRepository
                .Setup(r => r.UpdateAsync(otp))
                .ReturnsAsync(1);

            // Act
            var result = await _otpService.UpdateOneTimePassword(otp);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task UpdateOneTimePassword_WhenCodeDoesNotExist_Returns0()
        {
            // Arrange
            var otp = new OneTimePassword { Code = "123456" };

            _mockOtpRepository
                .Setup(r => r.CheckIfCodeExistsAsync("123456"))
                .ReturnsAsync(false);

            // Act
            var result = await _otpService.UpdateOneTimePassword(otp);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task RedeemOneTimePassword_ParentInvitesChild_BindsParentToChild()
        {
            // Arrange: Parent invites a child (ChildUser is set)
            var otp = new OneTimePassword
            {
                Code = "222222",
                GeneratedBy = 1,      // Parent
                UsedByUser = 2,       // Another parent
                Used = true,
                ChildUser = 3         // Child
            };

            _mockOtpRepository
                .Setup(r => r.CheckIfCodeExpiredAsync("222222"))
                .ReturnsAsync(false);

            _mockOtpRepository
                .Setup(r => r.GetFromCodeAsync("222222"))
                .ReturnsAsync(otp);

            _mockChildrenRepository
                .Setup(r => r.AddParentToChildAsync(2, 3))
                .ReturnsAsync(123);

            // Act
            var result = await _otpService.RedeemOneTimePassword("222222");

            // Assert
            Assert.Equal(123, result);
            _mockOtpRepository.Verify(r => r.DeleteAsync("222222"), Times.Once);
            _mockChildrenRepository.Verify(r => r.AddParentToChildAsync(2, 3), Times.Once);
        }

        [Fact]
        public async Task RedeemOneTimePassword_ParentInvitesParent_BindsChildToParent()
        {
            // Arrange
            var otp = new OneTimePassword
            {
                Code = "333333",
                GeneratedBy = 1,
                UsedByUser = 2,
                Used = true,
                ChildUser = null
            };

            _mockOtpRepository
                .Setup(r => r.CheckIfCodeExpiredAsync("333333"))
                .ReturnsAsync(false);

            _mockOtpRepository
                .Setup(r => r.GetFromCodeAsync("333333"))
                .ReturnsAsync(otp);

            _mockChildrenRepository
                .Setup(r => r.AddParentToChildAsync(1, 2))
                .ReturnsAsync(456);

            // Act
            var result = await _otpService.RedeemOneTimePassword("333333");

            // Assert
            Assert.Equal(456, result);
            _mockOtpRepository.Verify(r => r.DeleteAsync("333333"), Times.Once);
            _mockChildrenRepository.Verify(r => r.AddParentToChildAsync(1, 2), Times.Once);
        }
    }
}
