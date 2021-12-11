import styles from "../../styles/MobileMenu.module.scss";
import Link from "next/link";
import PropTypes from "prop-types";

const MobileMenu = ({menuItems, menuOpen, handleMenu}) => {
	return (
		<div className={`${styles.mobileMenu} ${menuOpen? styles.mobileMenuOpen : styles.mobileMenuClosed}`}>
			<button onClick={handleMenu} className={styles.exitIcon}><span>X</span></button>
			<ul className={styles.mobileMenuList}>
				{menuItems.map(item =>
					<Link key={item} href={`/${item.toLowerCase()}`}> 
						<li className={styles.mobileMenuItem} >
							<span>{item}</span>
						</li>
					</Link>
				)}
			</ul>
		</div>
	);
};

MobileMenu.propTypes = {
	menuItems: PropTypes.array,
	menuOpen: PropTypes.bool,
	handleMenu: PropTypes.func
};

export default MobileMenu;