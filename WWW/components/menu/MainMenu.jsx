import SearchBar from "./SearchBar";
import styles from "../../styles/MainMenu.module.scss";

export default function MainMenu() {
	return (
		<nav className={styles.navbar}>
			<ul>
				<button>Hamburger menu</button>
				<button>Dark Mode Toggle</button>
				<SearchBar />
			</ul>
		</nav>
	);
}