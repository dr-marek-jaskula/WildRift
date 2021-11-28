import logo from "../../resources/images/main_page/logo.svg";
import Image from "next/image";
import styles from "../../styles/Logo.module.scss";

const Logo = () => {
	return (
		<div className={styles.logoWrapper}>
			<Image className={styles.logoImg} src={logo} />
		</div>
	);
};

export default Logo;